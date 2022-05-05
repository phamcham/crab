using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitTaskAttack : MonoBehaviour, IUnitTask {
    public enum AttackState {
        MoveToEnemy,
        Attack,
        DoNothing
    }
    public bool IsTaskRunning { get; private set; } = false;
    // ====== public =========
    public PlayerUnit BaseUnit { get; private set; }
    public int catchRadius;
    public int attackRadius;
    public float reloadingTime;
    // ======= private ============
    private float curReloadingTime = 0;
    private float checkTime = 0;
    private bool isInRange = false;
    private bool initStep = false;
    private AttackState currentState = AttackState.DoNothing;
    private UnitMovement movement;
    private EnemyUnit targetEnemy;
    private EnemyUnit followToDieEnemy;
    //private Animation anim;
    private Dictionary<AttackState, Action> steps = new Dictionary<AttackState, Action>();
    private IUnitTask[] allTasks;
    // ======= const ===========
    const float checkInterval = 2f;
    // ====== method =======
    protected void Awake() {
        BaseUnit = GetComponent<PlayerUnit>();
        movement = GetComponent<UnitMovement>();
        allTasks = GetComponents<IUnitTask>();

        OnAwake();
    }
    protected abstract void OnAwake();
    protected void Start() {
        steps.Add(AttackState.MoveToEnemy, MoveToEnemyAction);
        steps.Add(AttackState.Attack, AttackAction);
    }
    private void MoveToEnemyAction() {
        if (initStep) {
            initStep = false;

        }

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
                movement.MoveToPosition(targetEnemy.transform.position);
            }
        }
        else { 
            EndDoTask();
        }
    }

    private void AttackAction() {
        if (initStep) {
            initStep = false;
        }

        if (targetEnemy) {
            float distance = Vector2.Distance(transform.position, targetEnemy.transform.position);
            if (distance > attackRadius) {
                SetState(AttackState.MoveToEnemy);
            }
            else {
                print("danh di");
                if (curReloadingTime <= 0) {
                    curReloadingTime = reloadingTime;
                    // attack
                    //anim.Play("crab attack");
                    OnAttack(targetEnemy);
                    //targetEnemy.GetComponent<IDamagable>()?.TakeDamage(BaseUnit.properties.damage);
                }
            }
        }
        else {
            EndDoTask();
        }
    }

    // attack theo cach cua tung child
    protected abstract void OnAttack(EnemyUnit enemyUnit);

    private void Update() {
        // always find nearest, even task is not started
        CheckEnemyAround();

        if (IsTaskRunning) {
            if (steps.TryGetValue(currentState, out Action action)){
                action?.Invoke();
                //print(currentStep);
            }
        }

        curReloadingTime = Mathf.Max(0, curReloadingTime - Time.deltaTime);
    }

    void CheckEnemyAround() {
        if (followToDieEnemy) {
            targetEnemy = followToDieEnemy;
            return;
        }
        
        targetEnemy = null;
        // get nearest enemy
        float checkDistance = float.MaxValue;
        if (checkTime > 0) {
            checkTime -= Time.deltaTime;
        }
        else {
            checkTime = checkInterval;
            // check enemy
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
        if (targetEnemy && !IsTaskRunning) {
            // TODO: dung het task
            // ...
            StartDoTask();
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
        // stop all tasks before do this task
        foreach (IUnitTask task in allTasks) {
            task.EndDoTask();
        }

        IsTaskRunning = true;
        SetState(AttackState.MoveToEnemy);
    }

    public void EndDoTask() {
        IsTaskRunning = false;
        targetEnemy = null;
        followToDieEnemy = null;
    }
}
