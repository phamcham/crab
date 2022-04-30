using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitShootable : MonoBehaviour {
    [SerializeField] int attackRadius;
    [SerializeField] int reloadingTime;
    public Unit BaseUnit { get; private set; }
    const float checkInterval = 1f;
    float checkTime = 0;
    float curReloadingTime = 0;
    UnitDamagable enemey;
    private void Awake() {
        BaseUnit = GetComponent<Unit>();
    }

    private void Update() {
        if (curReloadingTime < reloadingTime) {
            curReloadingTime += Time.deltaTime;
        }
        else {
            if (enemey) {
                curReloadingTime = 0;
                // check if old enemy still in range
                float distance = Vector2.Distance(transform.position, enemey.transform.position);
                if (distance < attackRadius) {
                    // attack enemy
                    Vector2 direction = (enemey.transform.position - transform.position).normalized;
                    Bullet bullet = BulletManager.GetBulletPooled();
                    bullet.transform.position = transform.position;
                    bullet.Shoot(BaseUnit, BaseUnit.properties.damage, direction, bullet.speed);
                }
                else {
                    // find other enemy
                    enemey = null;
                }
            }
            else {
                if (checkTime < checkInterval) {
                    checkTime += Time.deltaTime;
                }
                else {
                    checkTime = 0;
                    // check enemy
                    Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, attackRadius);
                    foreach (Collider2D collider in collider2Ds) {
                        if (collider.TryGetComponent(out UnitDamagable damagable)) {
                            if (damagable.BaseUnit.Team != BaseUnit.Team) {
                                // kill this
                                enemey = damagable;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
