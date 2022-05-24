using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskCloseQuartersCombat : UnitTaskAttack, ISaveObject<TaskCloseQuartersCombatSaveData> {
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

    public TaskCloseQuartersCombatSaveData GetSaveObjectData() {
        return new TaskCloseQuartersCombatSaveData() {
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
public struct TaskCloseQuartersCombatSaveData {
    public bool isRunning;
    public UnitTaskAttack.AttackState attackState;
    public int catchRadius;
    public int attackRadius;
    public float reloadingTime;
    public bool isInRange;
}