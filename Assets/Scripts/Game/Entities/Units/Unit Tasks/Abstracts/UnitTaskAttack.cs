using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class UnitTaskAttack : MonoBehaviour, IUnitTask {
    public enum AttackState {
        MoveToEnemy,
        Attack,
        DoNothing
    }
    public bool IsTaskRunning { get; private set; } = false;
    //public bool IsTaskRunning => throw new NotImplementedException();
    // ====== public =========
    public PlayerUnit BaseUnit { get; private set; }

    public int catchRadius;
    public int attackRadius;
    public float reloadingTime;
    // ======= private ============
    private float curReloadingTime = 0;
    private bool isInRange = false;
    private bool initStep = false;
    private AttackState currentState = AttackState.DoNothing;
    private UnitNavMovement movement;
    private EnemyUnit targetEnemy;
    private EnemyUnit followToDieEnemy;
    //private Animation anim;
    private Dictionary<AttackState, Action> steps = new Dictionary<AttackState, Action>();
    // ====== method =======
    Tween scanEnemiesTween;
    protected void Awake() {
        BaseUnit = GetComponent<PlayerUnit>();
        movement = GetComponent<UnitNavMovement>();

        OnAwake();
    }
    protected abstract void OnAwake();
    protected void Start() {
        steps.Add(AttackState.MoveToEnemy, MoveToEnemyAction);
        steps.Add(AttackState.Attack, AttackAction);

        scanEnemiesTween = DOVirtual.DelayedCall(2f, ScanEnemies).SetLoops(-1).Play();
    }
    private void MoveToEnemyAction() {
        if (initStep) {
            initStep = false;

        }

        //print("move : " + (targetEnemy ? "ok" : "null"));

        if (targetEnemy) {
            // follow enemy
            float distance = Vector2.Distance(transform.position, targetEnemy.transform.position);

            if (distance * 3 < attackRadius * 2) {
                // ke dich trong tam danh
                if (!isInRange) movement.StopMovement();
                isInRange = true;

                SetState(AttackState.Attack);
            }
            else {
                // ke dich ngoai tam ban
                isInRange = false;
                // duoi theo nooooo
                movement.Move(targetEnemy.transform.position);
            }
        }
    }

    private void AttackAction() {
        if (initStep) {
            initStep = false;
        }

        //print("attack : " + (targetEnemy ? "ok" : "null"));
        if (targetEnemy) {
            float distance = Vector2.Distance(transform.position, targetEnemy.transform.position);
            if (distance > attackRadius) {
                SetState(AttackState.MoveToEnemy);
            }
            else {
                if (curReloadingTime <= 0) {
                    //print("danh di: " + Time.realtimeSinceStartup + ", " + curReloadingTime);
                    curReloadingTime = reloadingTime;
                    // attack
                    //anim.Play("crab attack");
                    OnAttack(targetEnemy);
                    //targetEnemy.GetComponent<IDamagable>()?.TakeDamage(BaseUnit.properties.damage);
                }
            }
        }
    }

    // attack theo cach cua tung child
    protected abstract void OnAttack(EnemyUnit enemyUnit);

    private void Update() {

        if (IsTaskRunning) {
            if (steps.TryGetValue(currentState, out Action action)){
                action?.Invoke();
                //print(currentStep);
            }
        }

        curReloadingTime = Mathf.Max(0, curReloadingTime - Time.deltaTime);
    }

    private void ScanEnemies() {
        // always find nearest, even task is not started
        if (followToDieEnemy) {
            targetEnemy = followToDieEnemy;
        }
        else {
            float checkDistance = float.MaxValue;
            List<EnemyUnit> enemies = GetEnemies(catchRadius);
            if (enemies.Count > 0) {
                foreach (EnemyUnit enemy in enemies) {
                    float distance = Vector2.Distance(enemy.transform.position, transform.position);
                    if (!targetEnemy || distance < checkDistance) {
                        targetEnemy = enemy;
                        checkDistance = distance;
                    }
                }
            }
        }
        // phat hien ra ke dich
        if (targetEnemy) {
            StartDoTask();
        }
        else {
            EndDoTask();
        }
    }

    // duoi theo ke dich den chet

    private List<EnemyUnit> GetEnemies(float radius) {
        List<EnemyUnit> list = new List<EnemyUnit>();
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in collider2Ds) {
            if (collider.TryGetComponent(out EnemyUnit enemyUnit) && enemyUnit.TryGetComponent(out IDamagable damagable)) {
                if (enemyUnit.Team != BaseUnit.Team) {
                    list.Add(enemyUnit);
                    break;
                }
            }
        }
        return list;
    }
    public void SetFollowEnemy(EnemyUnit enemyUnit) {
        //print("danh di");
        //targetEnemy = enemyUnit;
        followToDieEnemy = enemyUnit;
        SetState(AttackState.MoveToEnemy);
    }

    public void SetState(AttackState state) {
        currentState = state;
        initStep = true;
    }

    public void StartDoTask() {
        if (IsTaskRunning) return;
        
        //print("found enemey");

        IsTaskRunning = true;
        SetState(AttackState.MoveToEnemy);
    }

    public void EndDoTask() {
        if (!IsTaskRunning) return;
        IsTaskRunning = false;
        targetEnemy = null;
        followToDieEnemy = null;
    }

    private void OnDestroy() {
        scanEnemiesTween.Kill();
    }
}
