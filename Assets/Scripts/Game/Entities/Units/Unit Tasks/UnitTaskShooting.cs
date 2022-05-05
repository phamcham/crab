using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskShooting : UnitTaskAttack {
    public int bulletSpeed;
    //private Animation anim;
    protected override void OnAwake() {
        //anim = GetComponent<Animation>();
    }

    protected override void OnAttack(EnemyUnit enemyUnit) {
        // shoot
        Vector2 direction = (enemyUnit.transform.position - transform.position).normalized;
        Bullet bullet = BulletManager.GetBulletPooled();
        bullet.transform.position = transform.position;
        float lifeTime = 1.0f * attackRadius / bulletSpeed;
        bullet.Shoot(new BulletProperties(BaseUnit.Team, bulletSpeed, BaseUnit.properties.damage, direction), lifeTime);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
