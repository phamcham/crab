using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public abstract class Unit : MonoBehaviour, IMoveable {
    public BoundsInt area = new BoundsInt(Vector3Int.zero, Vector3Int.one);
    protected Vector2 TargetPosition { get; set; }
    protected UnitProperties Properties { get; set; }
    private Vector2[] path;
    private int targetIndex;
    private Rigidbody2D rb;
    private const float requestPathInterval = 0.5f;
    private float time = 0;
    private bool isPathFound = false;
    private Vector2Int? prevIndex = null;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual void Update() {
        if (time <= 0) {
            if (!isPathFound) {
                AStarPathRequestManager.RequestPath(new PathRequest(transform.position, TargetPosition, OnPathFound));
                isPathFound = true;
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

    public void OnPathFound(Vector2[] newPath, PathResultType pathSuccessful) {
        time = requestPathInterval;
        isPathFound = false;
        StopCoroutine(nameof(FollowPath));
        if (pathSuccessful != PathResultType.NotFound) {
            path = newPath;
            targetIndex = 0;
            StartCoroutine(nameof(FollowPath));
        }
        else{
            path = null;
        }
    }

    private IEnumerator FollowPath() {
        Vector3 currentWaypoint = path[0];

        while (true) {
            if (transform.position == currentWaypoint) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            float speed = Properties.moveSpeed;
            MovePosition(Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.fixedDeltaTime));

            OnMove(currentWaypoint - transform.position);
            yield return new WaitForFixedUpdate();
        }
    }

    protected virtual void OnDrawGizmos() {
        if (Application.isPlaying){
            if (path != null) {
                for (int i = targetIndex; i < path.Length; i++) {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(path[i], 0.3f);

                    if (i == targetIndex) {
                        Gizmos.DrawLine(transform.position, path[i]);
                    } else {
                        Gizmos.DrawLine(path[i - 1], path[i]);
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