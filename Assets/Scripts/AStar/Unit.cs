using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public abstract class Unit : MonoBehaviour, IMoveable {
    public BoundsInt area = new BoundsInt(Vector3Int.zero, Vector3Int.one);
    private Vector2 _targetPosition;
    public Vector2 TargetPosition {
        get => _targetPosition;
        set {
            if (_targetPosition != value){
                // chuyen thanh notfound ngay vi van de dong bo
                PathResultType = PathResultType.NotFound;
            }
            _targetPosition = value;
        }
    }
    private Vector2 prevTargetPosition;
    public UnitProperties Properties { get; set; }
    private Vector2[] followingSimplifyPath;
    private Vector2[] followingFullPath;
    private int targetIndex;
    private Rigidbody2D rb;
    private const float requestInterval = 0.5f;
    private float time = 0;
    private bool isPathFound = false;
    private Vector2Int? prevIndex = null;
    public PathResultType PathResultType { get; private set; }

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual void Update() {
        if (time <= 0) {
            if (!isPathFound) {
                // TODO: after long interval, check current path to new request path,
                // use new request path when cost big enough

                bool shouldRequestNew = true;
                //print(prevTargetPosition +", "+ TargetPosition);
                if (prevTargetPosition != TargetPosition){
                    // target had move
                    prevTargetPosition = TargetPosition;
                }
                else {
                    // target keep pos
                    if (followingFullPath != null){
                        shouldRequestNew = false;
                        foreach (Vector2 pos in followingFullPath){
                            AStarNode node = AStarGrid.current.NodeFromWorldPoint(pos);
                            if (!node.walkable){
                                shouldRequestNew = true;
                                break;
                            }
                        }
                    }
                }

                if (shouldRequestNew){
                    //Debug.Log("req new path");
                    AStarPathRequestManager.RequestPath(new PathRequest(transform.position, TargetPosition, OnPathFound));
                    isPathFound = true;
                }
                else{
                    // keep prev path
                    //print("keep prev path");
                    time = requestInterval;
                    isPathFound = false;
                }
            }
        } else {
            time -= Time.deltaTime;
        }

        // TODO: thay vi transform.positon, chuyen thanh y dinh can di chuyen chu kp vi tri hien tai
        AStarNode currentNode = AStarGrid.current.NodeFromWorldPoint(transform.position);
        Vector2Int currentGridIndex = currentNode.gridIndex;

        if (prevIndex == null){
            prevIndex = currentGridIndex;
        }
        else{
            if (prevIndex != currentGridIndex){
                // xoa previndex obstacle
                AStarGrid.current.DeleteTempObstacle(prevIndex.Value);
                // them gridindex obstacle
                AStarGrid.current.AddTempObstacle(currentGridIndex);
                prevIndex = currentGridIndex;
            }
        }
    }

    public void OnPathFound(Vector2[] fullPath, Vector2[] simplifyPath, PathResultType pathStatus) {
        time = requestInterval;
        isPathFound = false;
        PathResultType = pathStatus;
        StopCoroutine(nameof(FollowPath));
        //Debug.Log(pathStatus);
        if (pathStatus == PathResultType.Found || pathStatus == PathResultType.NotEnough) {
            followingFullPath = fullPath;
            followingSimplifyPath = simplifyPath;
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
            time = requestInterval;
        }
    }

    private IEnumerator FollowPath() {
        Vector3 currentWaypoint = followingSimplifyPath[0];
        var _waitForFixedUpdate = new WaitForFixedUpdate();
        while (true) {
            if (followingSimplifyPath != null && followingSimplifyPath.Length > 0) {
                //Debug.Log($"({transform.position.x}, {transform.position.y}) ({currentWaypoint.x}, {currentWaypoint.y})");
                if (transform.position == currentWaypoint) {
                    targetIndex++;
                    if (targetIndex >= followingSimplifyPath.Length) {
                        //Debug.Log(name + " pathfinding completed");
                        // BUG: cai nay chi la di het duong chu khong phai la da hoan thanh
                        if (PathResultType == PathResultType.Found){
                            PathResultType = PathResultType.IsCompleted;
                        }
                        yield break;
                    }
                    currentWaypoint = followingSimplifyPath[targetIndex];
                }

                float speed = Properties.moveSpeed;
                Vector2 position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.fixedDeltaTime);
                //print($"position: ({position.x}, {position.y})");
                MovePosition(position);

                OnMove(currentWaypoint - transform.position);
                yield return _waitForFixedUpdate;
            }
            else{
                print("path null");
                PathResultType = PathResultType.NotFound;
                yield break;
            }
        }
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

    public void MovePosition(Vector2 position) {
        //Debug.Log($"move: ({position.x}, {position.y})");
        rb.MovePosition(position);
    }
    public abstract void OnMove(Vector2 direction);
}

public class UnitProperties{
    public Team team;
    public int healthPoint;
    public float moveSpeed;
    public int damage;

    public UnitProperties(Team team, int healthPoint, int damage, float moveSpeed){
        this.team = team;
        this.healthPoint = healthPoint;
        this.damage = damage;
        this.moveSpeed = moveSpeed;
    }
}