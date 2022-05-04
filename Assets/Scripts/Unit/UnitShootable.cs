using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitShootable : MonoBehaviour {
    public int catchRadius;
    public int attackRadius;
    public float reloadingTime;
    public int bulletSpeed;
    private float curReloadingTime = 0;
    public PlayerUnit BaseUnit { get; private set; }
    private UnitMovement movement;
    const float checkInterval = 2f;
    private float checkTime = 0;
    private EnemyUnit targetEnemy;
    private EnemyUnit followToDieEnemy;
    bool isInRange = false;
    private void Awake() {
        BaseUnit = GetComponent<PlayerUnit>();
        movement = GetComponent<UnitMovement>();
    }

    private void Update() {
        FindNearestEnemy();
        FollowAndAttack();
        
        curReloadingTime = Mathf.Max(0, curReloadingTime - Time.deltaTime);
    }

    void FindNearestEnemy() {
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
    }

    void FollowAndAttack() {
        if (targetEnemy) {
            // follow enemy
            float distance = Vector2.Distance(transform.position, targetEnemy.transform.position);

            if (distance * 3 < attackRadius * 2) {
                // ke dich trong tam danh
                if (!isInRange) movement.StopMovement();
                isInRange = true;

                if (curReloadingTime <= 0) {
                    curReloadingTime = reloadingTime;
                    // shoot
                    Vector2 direction = (targetEnemy.transform.position - transform.position).normalized;
                    Bullet bullet = BulletManager.GetBulletPooled();
                    bullet.transform.position = transform.position;
                    float lifeTime = 1.0f * attackRadius / bulletSpeed;
                    bullet.Shoot(new BulletProperties(BaseUnit.Team, bulletSpeed, BaseUnit.properties.damage, direction), lifeTime);
                }
            }
            else {
                // ke dich ngoai tam ban
                isInRange = false;
                // duoi theo nooooo
                movement.MoveToPosition(targetEnemy.transform.position);
            }
        }
    }

    // duoi theo ke dich den chet
    public void SetFollowEnemy(EnemyUnit enemyUnit) {
        //print("danh di");
        //targetEnemy = enemyUnit;
        followToDieEnemy = enemyUnit;
    }
    public void StopShooting() {
        //print("k danh nua");
        targetEnemy = null;
    }

    private List<EnemyUnit> GetEnemies(float radius) {
        List<EnemyUnit> list = new List<EnemyUnit>();
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in collider2Ds) {
            if (collider.TryGetComponent(out EnemyUnit damagable)) {
                if (damagable.Team != BaseUnit.Team) {
                    list.Add(damagable);
                    break;
                }
            }
        }
        return list;
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
