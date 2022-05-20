using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskCloseQuartersCombat : UnitTaskAttack {
    private AnimationManager anim;
    private const string CRAB_ATTACK = "crab attack";
    protected override void OnAwake() {
        anim = GetComponent<AnimationManager>();
    }
    protected override void OnAttack(Entity enemy) {
        anim.Play(CRAB_ATTACK);
        SlashEffect effect = SlashEffectManager.GetObjectPooled();
        effect.transform.position = transform.position;
        enemy.GetComponent<IDamagable>()?.TakeDamage(BaseUnit.properties.damage);
    }
}
