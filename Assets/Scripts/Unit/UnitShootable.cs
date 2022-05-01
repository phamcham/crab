using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitShootable : MonoBehaviour {
    public int attackRadius;
    public float reloadingTime;
    public int bulletSpeed;
    private float curReloadingTime = 0;
    public Unit BaseUnit { get; private set; }
    private UnitShootable shootable;
    private UnitMovement movement;
    const float checkInterval = 2f;
    private float checkTime = 0;
    private FollowEnemyState followEnemyState;
    private EnemyUnit targetEnemy;
    bool isInRange = false;
    private void Awake() {
        BaseUnit = GetComponent<Unit>();
        shootable = GetComponent<UnitShootable>();
        movement = GetComponent<UnitMovement>();
    }

    private void Update() {
        if (!targetEnemy) {
            // neu khong tim thay ke dich
            if (checkTime < checkInterval) {
                checkTime += Time.deltaTime;
            }
            else {
                checkTime = 0;
                // check enemy
                List<EnemyUnit> enemies = GetEnemies();
                if (enemies.Count > 0) {
                    SetFollowEnemy(enemies[0], FollowEnemyState.Defense);
                }
            }
        }
        else {
            // follow enemy
            float distance = Vector2.Distance(transform.position, targetEnemy.transform.position);

            if (distance < attackRadius) {
                // ke dich trong tam danh
                if (!isInRange) movement.StopMovement();
                isInRange = true;

                if (curReloadingTime >= reloadingTime) {
                    curReloadingTime = 0;
                    // shoot
                    Vector2 direction = (targetEnemy.transform.position - transform.position).normalized;
                    Bullet bullet = BulletManager.GetBulletPooled();
                    bullet.transform.position = transform.position;
                    float lifeTime = 1.0f * shootable.attackRadius / bulletSpeed;
                    bullet.Shoot(new BulletProperties(BaseUnit.Team, bulletSpeed, BaseUnit.properties.damage, direction), lifeTime);
                }
                else {
                    curReloadingTime += Time.deltaTime;
                }
            }
            else {
                // ke dich ngoai tam ban
                isInRange = false;
                
                if (followEnemyState == FollowEnemyState.Hunt) {
                    // duoi theo nooooo
                    movement.MoveToPosition(targetEnemy.transform.position);
                }
                else if (followEnemyState == FollowEnemyState.Defense) {
                    print("k duoi nua");
                    targetEnemy = null;
                    movement.StopMovement();
                }
            }
        }
    }

    // duoi theo ke dich den chet
    public void SetFollowEnemy(EnemyUnit enemyUnit, FollowEnemyState state) {
        print("danh di");
        targetEnemy = enemyUnit;
        followEnemyState = state;
    }
    public void StopAttacking() {
        print("k danh nua");
        targetEnemy = null;
    }

    private List<EnemyUnit> GetEnemies() {
        List<EnemyUnit> list = new List<EnemyUnit>();
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, shootable.attackRadius);
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

    public enum FollowEnemyState {
        Hunt, // tieu diet den chet
        Defense, // thay la don, chay thi thoi
    }
}
