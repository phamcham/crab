using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public class AStarPathRequestManager : MonoBehaviour {
    private Queue<PathResult> results = new Queue<PathResult>();

    private static AStarPathRequestManager instance;
    private AStarPathfinding pathfinding;

    private void Awake() {
        instance = this;
        pathfinding = GetComponent<AStarPathfinding>();
    }

    private void Update() {
        if (results.Count > 0) {
            int itemsInQueue = results.Count;
            lock (results) {
                for (int i = 0; i < itemsInQueue; i++) {
                    PathResult result = results.Dequeue();
                    result.callback?.Invoke(result.fullPath, result.simplifyPath, result.result);
                }
            }
        }
    }

    public static void RequestPath(PathRequest request) {
        ThreadStart threadStart = delegate {
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }

    public void FinishedProcessingPath(PathResult result) {
        lock (results) {
            results.Enqueue(result);
        }
    }
}

public struct PathResult {
    public Vector2[] fullPath;
    public Vector2[] simplifyPath;
    public PathResultType result;
    public PathRequest.Callback callback;

    public PathResult(Vector2[] fullPath, Vector2[] simplifyPath, PathResultType success, PathRequest.Callback callback) {
        this.fullPath = fullPath;
        this.simplifyPath = simplifyPath;
        this.result = success;
        this.callback = callback;
    }
}

public struct PathRequest {
    public Vector2 pathStart;
    public Vector2 pathEnd;
    public Callback callback; // simplify path, full path

    public PathRequest(Vector2 _start, Vector2 _end, Callback _callback) {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }

    public delegate void Callback(Vector2[] fullPath, Vector2[] simplifyPath, PathResultType type);
}

public enum PathResultType{
    Found,
    NotEnough,
    NotFound,
    IsCompleted
}