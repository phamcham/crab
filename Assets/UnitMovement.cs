using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Unit))]
public class UnitMovement : MonoBehaviour {
    private Unit unit;
    private Rigidbody2D rb;
    public Vector2 TargetPosition {get; private set;}
    private Vector2 prevTargetPosition = new Vector2(int.MaxValue, int.MinValue);
    private Vector2[] followingSimplifyPath;
    private Vector2[] followingFullPath;
    private int targetIndex;
    private const float requestInterval = 0.5f;
    private float time = 0;
    private bool isPathFound = false;
    //private Vector2Int prevIndex = new Vector2Int(int.MaxValue, int.MinValue);
    public PathResultType PathResultType { get; private set; }
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        unit = GetComponent<Unit>();
    }
    private void Update() {
        if (time <= 0) {
            if (!isPathFound && PathResultType != PathResultType.IsCompleted) {
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
                    time = requestInterval;
                    isPathFound = false;
                }
            }
        } else {
            time -= Time.deltaTime;
        }
    }

    public void OnPathFound(Vector2[] fullPath, Vector2[] simplifyPath, PathResultType pathStatus) {
        time = requestInterval;
        isPathFound = false;
        PathResultType = pathStatus;
        prevTargetPosition = TargetPosition;
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
                        //float distance = Vector2.Distance(transform.position, TargetPosition);
                        //Debug.Log(distance);
                        if (PathResultType == PathResultType.Found){
                            PathResultType = PathResultType.IsCompleted;
                        }
                        yield break;
                    }
                    currentWaypoint = followingSimplifyPath[targetIndex];
                }

                float speed = unit.Properties.moveSpeed;
                Vector2 position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.fixedDeltaTime);
                
                TryTranslateToPosition(position);

                yield return _waitForFixedUpdate;
            }
            else {
                print("path null");
                PathResultType = PathResultType.NotFound;
                yield break;
            }
        }
    }

    void TryTranslateToPosition(Vector2 position){
        // TODO: thay vi transform.positon, chuyen thanh y dinh can di chuyen chu kp vi tri hien tai
        // Vector2Int currentGridIndex = AStarGrid.current.NodeFromWorldPoint(transform.position).gridIndex;
        // Vector2Int nextGridIndex = AStarGrid.current.NodeFromWorldPoint(position).gridIndex;
        // if (nextGridIndex != currentGridIndex){
        //     // unit da di chuyen den mot cell moi
        //     // TODO: kiem tra cell do da bi chiem boi 1 unit nao chua, hard code
        //     var units = FindObjectsOfType<Unit>();
        //     for (int i = 0; i < units.Length; i++){
        //         Vector2Int check = AStarGrid.current.NodeFromWorldPoint(units[i].transform.position).gridIndex;
        //         if (check == nextGridIndex){
        //             // vi tri chuan bi di chuyen da bi chiem
        //         }
        //     }

        //     AStarGrid.current.DeleteTempObstacle(currentGridIndex);
        //     AStarGrid.current.AddTempObstacle(nextGridIndex);
        // }
        //print($"position: ({position.x}, {position.y})");
        rb.MovePosition(position);
        
    }

    // bool RigidMove(Vector2 position){
    //     Vector2 direction = (position - (Vector2)transform.position).normalized;
    //     float distance = Vector2.Distance(position, transform.position);
    //     List<RaycastHit2D> results = new List<RaycastHit2D>();
    //     int count = rb.Cast(direction, results, distance);
    //     if (count == 0){
    //         //Debug.Log(direction);
    //         // No collisions
    //         rb.MovePosition(position);
    //         return true;
    //     }
    //     else {
    //         foreach (RaycastHit2D hit in results){
    //             if (hit.collider != null && hit.collider.gameObject != null){
    //                 if (hit.collider.TryGetComponent<Unit>(out Unit unit)){
                        
    //                 }
    //             }
    //         }
    //         return false;
    //     }
    // }

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
        }
        TargetPosition = position;
    }
}
