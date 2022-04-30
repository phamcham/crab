using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitShootable : MonoBehaviour {
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] int attackRadius;
    [SerializeField] int reloadingTime;
    private Unit unit;
    //const float checkInterval = 1f;
    //float checkTime = 0;
    float curReloadingTime = 0;
    UnitDamagable enemey;
    private void Awake() {
        unit = GetComponent<Unit>();
    }

    private void Update() {
        if (curReloadingTime < reloadingTime) {
            curReloadingTime += Time.deltaTime;
        }
        else{
            curReloadingTime = 0;
            if (enemey) {
                // check if old enemy still in range
                float distance = Vector2.Distance(transform.position, enemey.transform.position);
                if (distance < attackRadius) {
                    // attack enemy
                    Vector2 direction = (enemey.transform.position - transform.position).normalized;
                    Bullet bullet = BulletManager.GetBulletPooled();
                    bullet.transform.position = transform.position;
                    bullet.Shoot(unit.properties.damage, direction, bullet.speed);
                }
                else {
                    // find other enemy
                    Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, attackRadius);
                    foreach (Collider2D collider2D in collider2Ds) {
                        if (collider2D.TryGetComponent(out UnitDamagable damagable)) {
                            Unit target = damagable.BaseUnit;
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
