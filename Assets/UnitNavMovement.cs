using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Unit), typeof(UnitTaskManager))]
public class UnitNavMovement : MonoBehaviour {
    public Unit BaseUnit { get; private set; }
    public Vector2 TargetPosition { get; private set; }
    private Animation anim;
    private UnitTaskManager taskManager;
    private NavMeshAgent navMeshAgent;
    private NavMeshModifier navMeshModifier;
    bool isStopped = true;
    private void Awake() {
        BaseUnit = GetComponent<Unit>();
        anim = GetComponent<Animation>();
        taskManager = GetComponent<UnitTaskManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshModifier = GetComponent<NavMeshModifier>();
    }

    private void Start() {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        navMeshAgent.isStopped = true;
    }

    private void Update() {
        navMeshAgent.speed = BaseUnit.properties.moveSpeed;
        if (!isStopped && IsReachedDestination() && taskManager.IsIdle()) {
            anim.Play("crab idle");
            isStopped = true;
        }
        if (isStopped) {
            navMeshAgent.avoidancePriority = 10;
        }
        else {
            navMeshAgent.avoidancePriority = 50;
        }
    }
    private void LateUpdate() {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public bool IsReachedDestination() {
         // Check if we've reached the destination
        if (!navMeshAgent.pathPending) {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f) {
                    // Done
                    return true;
                }
            }
        }
        return false;
    }

    public void StopMovement() {
        navMeshAgent.isStopped = true;
        isStopped = true;
        anim.Play("crab idle");
    }

    protected virtual void OnDrawGizmos() {
        if (Application.isPlaying){
            //navMeshAgent.path.corners
        }
    }

    public void Move(Vector2 position) {
        TargetPosition = position;
        
        navMeshAgent.isStopped = false;
        isStopped = false;
        anim.Play("crab move");

        var agentDrift = 0.0001f; // minimal
        var driftPos = TargetPosition + (Vector2)(agentDrift * Random.insideUnitCircle.normalized);
        navMeshAgent.SetDestination(driftPos);
    }
}
