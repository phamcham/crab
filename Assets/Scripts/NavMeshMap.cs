using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMap : MonoBehaviour {
    public static NavMeshMap current { get; private set; }
    [SerializeField] NavMeshSurface surface;
    private void Awake() {
        current = this;
    }
    private void Start() {
        surface.BuildNavMeshAsync();
    }
    public bool IsWalkable(Vector2 position) {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) {
            return true;
        }
        return false;
    }
    public void UpdateNavMesh() {
        StopCoroutine(nameof(UpdateNavMeshOnNextFrame));
        StartCoroutine(nameof(UpdateNavMeshOnNextFrame));
    }
    IEnumerator UpdateNavMeshOnNextFrame() {
        yield return null;
        print("update navmesh");
        for (int i = 0; i < 5; i++) {
            surface.UpdateNavMesh(surface.navMeshData);
            yield return null;
        }
    }
}
