using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskShooting : UnitTaskAttack, ISaveObject<TaskShootingSaveData> {
    public int bulletSpeed;
    //private Animation anim;
    protected override void OnAwake() {
        //anim = GetComponent<Animation>();
    }

    protected override void OnAttack(Entity enemy) {
        // shoot
        Vector2 direction = (enemy.transform.position - transform.position).normalized;
        Bullet bullet = BulletManager.GetObjectPooled();
        bullet.transform.position = transform.position;
        float lifeTime = 1.0f * attackRadius / bulletSpeed;
        bullet.Shoot(new BulletProperties(BaseUnit.Team, bulletSpeed, BaseUnit.properties.damage, direction), lifeTime);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public TaskShootingSaveData GetSaveObjectData() {
        return new TaskShootingSaveData() {
            isRunning = IsTaskRunning,
            attackState = base.currentState,
            catchRadius = base.catchRadius,
            attackRadius = base.attackRadius,
            reloadingTime = base.reloadingTime,
            isInRange = base.isInRange
        };
    }
}

[System.Serializable]
public struct TaskShootingSaveData {
    public bool isRunning;
    public UnitTaskAttack.AttackState attackState;
    public int catchRadius;
    public int attackRadius;
    public float reloadingTime;
    public bool isInRange;
}