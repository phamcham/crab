using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyUnit : Unit {
    public override Team Team => Team.DefaultEnemy;

    protected void Awake(){
        properties.curHealthPoint = properties.maxHealthPoint;
        OnAwake();
    }
    protected abstract void OnAwake();
    protected void Start(){
        OnStart();
    }
    protected abstract void OnStart();
    public Sprite GetSprite() {
        return spriteRenderer.sprite;
    }
    protected void OnDestroy() {
        OnUnitDestroy();
    }
    protected abstract void OnUnitDestroy();
}
