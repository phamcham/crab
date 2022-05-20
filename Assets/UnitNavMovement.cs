using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Unit), typeof(UnitTaskManager))]
public class UnitNavMovement : MonoBehaviour {
    public Unit BaseUnit { get; private set; }
    public Vector2 TargetPosition { get; private set; }
    public Building TargetBuilding { get; private set; }
    private AnimationManager anim;
    private const string CRAB_MOVE = "crab move";
    private const string CRAB_IDLE = "crab idle";
    private UnitTaskManager taskManager;
    private NavMeshAgent navMeshAgent;
    private NavMeshModifier navMeshModifier;
    bool isStopped = true;
    DG.Tweening.Tween moveRandomTween = null;
    private void Awake() {
        BaseUnit = GetComponent<Unit>();
        anim = GetComponent<AnimationManager>();
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

        if (!isStopped) {
            //print(navMeshAgent.velocity);
            if (navMeshAgent.velocity == Vector3.zero && taskManager.IsIdle() && IsReachedDestination()) {
                isStopped = true;
                anim.Play(CRAB_IDLE);
            }
        }

        if (isStopped) {
            navMeshAgent.avoidancePriority = 10;
            if (moveRandomTween == null) {
                float time = Random.Range(6f, 9f);
                moveRandomTween = DOVirtual.DelayedCall(time, () => {
                    Vector2 delta = Random.insideUnitCircle * 3f;
                    MoveToPosition((Vector2)transform.position + delta);
                }).Play();
            }
        }
        else {
            navMeshAgent.avoidancePriority = 50;
            if (moveRandomTween != null) {
                moveRandomTween.Kill();
                moveRandomTween = null;
            }
        }
    }
    private void LateUpdate() {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public bool IsReachedDestination() {
         // Check if we've reached the destination
        if (!navMeshAgent.pathPending) {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                float distance = Vector2.Distance(transform.position, TargetPosition);
                if (distance <= 2.5f) {
                    if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f) {
                        // Done
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void StopMovement() {
        navMeshAgent.isStopped = true;
        isStopped = true;
        //anim.Play(CRAB_IDLE);
    }

    protected virtual void OnDrawGizmos() {
        if (Application.isPlaying){
            //navMeshAgent.path.corners
        }
    }

    public void MoveToPosition(Vector2 position) {
        navMeshAgent.isStopped = false;
        isStopped = false;
        anim.Play(CRAB_MOVE);

        var agentDrift = 0.0001f; // minimal
        var driftPos = position + (Vector2)(agentDrift * Random.insideUnitCircle.normalized);
        navMeshAgent.SetDestination(TargetPosition = driftPos);
    }
}
