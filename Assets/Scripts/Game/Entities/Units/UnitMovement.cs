using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;

[RequireComponent(typeof(Unit), typeof(UnitTaskManager))]
public class UnitMovement : MonoBehaviour {
    public Unit BaseUnit { get; private set; }
    public Vector2 TargetPosition { get; private set; }
    private Vector2 prevTargetPosition = new Vector2(int.MaxValue, int.MinValue);
    private Vector2[] followingSimplifyPath;
    private Vector2[] followingFullPath;
    private int targetIndex;
    private const float requestInterval = 0.5f;
    private float curRequestNewPathTime = 0;
    private bool isPathFound = false;
    //private Vector2Int prevIndex = new Vector2Int(int.MaxValue, int.MinValue);
    public PathResultType PathResultType { get; private set; }
    public bool startMoving { get; private set; } = false;
    private Animation anim;
    private float moveRandomWhenIdleTime = 10f;
    float curMoveRandomWhenIdle = 0;
    float curMoveRandomDistance = 3;
    private UnitTaskManager taskManager;
    private void Awake() {
        BaseUnit = GetComponent<Unit>();
        anim = GetComponent<Animation>();
        taskManager = GetComponent<UnitTaskManager>();
    }

    private void Update() {
        RequestNewPathAndMoveInterval();
        MoveRandomInterval();
    }

    private void RequestNewPathAndMoveInterval() {
        if (curRequestNewPathTime <= 0) {
            RequestNewPathAndMove();
        }
        curRequestNewPathTime -= Time.deltaTime;
    }

    private void MoveRandomInterval() {
        if (!startMoving && taskManager.IsIdle()) {
            if (curMoveRandomWhenIdle >= moveRandomWhenIdleTime) {
                Vector2 next = transform.position + Random.onUnitSphere * curMoveRandomDistance;
                AStarPathRequestManager.RequestPath(new PathRequest(transform.position, next, (fullPath, simplifyPath, type) => {
                    curMoveRandomWhenIdle = 0;
                    moveRandomWhenIdleTime = Random.Range(15, 20);
                    curMoveRandomDistance = Random.Range(3, 5);
                    if (type != PathResultType.NotFound) {
                        MoveToPosition(next);
                    }
                }));
            }
            else {
                curMoveRandomWhenIdle += Time.deltaTime;
            }
        }
        else {
            curMoveRandomWhenIdle = 0;
        }
    }

    private void RequestNewPathAndMove() {
        if (!isPathFound && startMoving && PathResultType != PathResultType.IsCompleted) {
            // TODO: after long interval, check current path to new request path,
            // use new request path when cost big enough

            bool shouldRequestNew = true;
            if (prevTargetPosition == TargetPosition) {
                // target keep pos
                shouldRequestNew = false;
                if (followingFullPath != null){
                    foreach (Vector2 pos in followingFullPath){
                        AStarNode node = AStarGrid.current.NodeFromWorldPoint(pos);
                        if (!node.walkable){
                            shouldRequestNew = true;
                            break;
                        }
                    }
                }
            }

            //Debug.Log($"curupdate from {transform.position} to {TargetPosition}" );
            if (shouldRequestNew){
                //  Debug.Log($"req new path from {transform.position} to {TargetPosition}" );
                AStarPathRequestManager.RequestPath(new PathRequest(transform.position, TargetPosition, OnPathFound));
                isPathFound = true;
            }
            else{
                // keep prev path
                //print("keep prev path");
                curRequestNewPathTime = requestInterval;
                isPathFound = false;
            }
        }
    }

    public void OnPathFound(Vector2[] fullPath, Vector2[] simplifyPath, PathResultType pathStatus) {
        curRequestNewPathTime = requestInterval;
        isPathFound = false;
        PathResultType = pathStatus;
        prevTargetPosition = TargetPosition;
        StopCoroutine(nameof(FollowPath));
        //Debug.Log(pathStatus);
        if (pathStatus == PathResultType.Found || pathStatus == PathResultType.NotEnough) {
            followingFullPath = fullPath;
            if (followingFullPath.Length >= 1) {
                followingFullPath[followingFullPath.Length - 1] = TargetPosition;
            }

            followingSimplifyPath = simplifyPath;
            if (followingSimplifyPath.Length >= 1) {
                followingSimplifyPath[followingSimplifyPath.Length - 1] = TargetPosition;
            }

            targetIndex = 0;
            if (followingSimplifyPath != null){
                StartCoroutine(nameof(FollowPath));
            }
        }
        else{
            followingSimplifyPath = null;
            followingFullPath = null;
            // khong tim thay duong thi delay lau hon bth 1 chut
            // cause: thuong neu k tim thay duong thi sau do cung khong thay duong
            curRequestNewPathTime = requestInterval;
        }
    }

    private IEnumerator FollowPath() {
        Vector2 currentWaypoint = followingSimplifyPath[0];
        // float randomStopTime = Random.Range(2, 6);
        // float randomStopDuration = Random.Range(0.3f, 1);
        while (true) {
            // TODO: kiem tra theo chu ky co overlap unit nao khong

            if (followingSimplifyPath != null && followingSimplifyPath.Length > 0) {
                if ((Vector2)transform.position == currentWaypoint) {
                    targetIndex++;
                    if (targetIndex >= followingSimplifyPath.Length) {
                        if (PathResultType == PathResultType.Found){
                            //Debug.Log("xong : " + TargetPosition);
                            PathResultType = PathResultType.IsCompleted;
                            anim.Stop("crab move");
                            anim.Play("crab idle");
                            startMoving = false;
                        }
                        yield break;
                    }
                    currentWaypoint = followingSimplifyPath[targetIndex];
                }

                anim.Play("crab move");
                float speed = BaseUnit.properties.moveSpeed;
                Vector2 position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                
                TryTranslateToPosition(position);

                yield return null;
            }
            else {
                PathResultType = PathResultType.NotFound;
                yield break;
            }
        }
    }

    void TryTranslateToPosition(Vector2 position){
        transform.position = position;
        //print("2: " + rb.position.PCToString());
    }

    protected virtual void OnDrawGizmos() {
        if (Application.isPlaying){
            if (followingSimplifyPath != null) {
                for (int i = targetIndex; i < followingSimplifyPath.Length; i++) {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(followingSimplifyPath[i], 0.3f);

                    if (i == targetIndex) {
                        Gizmos.DrawLine(transform.position, followingSimplifyPath[i]);
                    } else {
                        Gizmos.DrawLine(followingSimplifyPath[i - 1], followingSimplifyPath[i]);
                    }
                }
                
            }
            AStarNode currentNode = AStarGrid.current.NodeFromWorldPoint(transform.position);
            Vector2 position = currentNode.worldPosition;
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(position, Vector3.one);
        }
    }

    public void MoveToPosition(Vector2 position){
        if (TargetPosition != position){
            // chuyen thanh notfound ngay vi van de dong bo
            PathResultType = PathResultType.NotFound;
            startMoving = true;
            TargetPosition = position;
        }
    }

    public void StopMovement() {
        anim.Stop("crab move");
        anim.Play("crab idle");
        TargetPosition = transform.position;
        startMoving = false;
        StopCoroutine(nameof(FollowPath));
    }

    public void HasPathToPosition(System.Action<bool> callback) {
        AStarPathRequestManager.RequestPath(new PathRequest(transform.position, TargetPosition, (fullPath, simplifyPath, pathStatus) => {
            bool hasPath = (pathStatus != PathResultType.NotFound);
            callback?.Invoke(hasPath); 
        }));
    }
}
