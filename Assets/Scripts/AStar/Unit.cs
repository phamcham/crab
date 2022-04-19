using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public abstract class Unit : MonoBehaviour, IMoveable {
    protected Vector2 TargetPosition { get; set; }
    private Vector2[] path;
    private int targetIndex;
    private Rigidbody2D rb;
    private const float requestPathInterval = 0.5f;
    private float time = 0;
    private bool isPathFound = false;

    protected UnitProperties Properties { get; set; }

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
    }

    public void OnPathFound(Vector2[] newPath, bool pathSuccessful) {
        time = requestPathInterval;
        isPathFound = false;
        if (pathSuccessful) {
            path = newPath;
            targetIndex = 0;
            StopCoroutine(nameof(FollowPath));
            StartCoroutine(nameof(FollowPath));
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
    }

    public void MovePosition(Vector2 position) {
        rb.MovePosition(position);
    }
    public abstract void OnMove(Vector2 direction);
}

public class UnitProperties{
    public int healthPoint;
    public float moveSpeed;
    public int damage;

    public UnitProperties(int healthPoint, int damage, float moveSpeed){
        this.healthPoint = healthPoint;
        this.damage = damage;
        this.moveSpeed = moveSpeed;
    }
}