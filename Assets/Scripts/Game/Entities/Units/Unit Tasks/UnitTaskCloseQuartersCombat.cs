using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskCloseQuartersCombat : UnitTaskAttack {
    private Animation anim;
    protected override void OnAwake() {
        anim = GetComponent<Animation>();
    }
    protected override void OnAttack(EnemyUnit enemyUnit) {
        anim.Play("crab attack");
        enemyUnit.GetComponent<IDamagable>()?.TakeDamage(BaseUnit.properties.damage);
    }

}
