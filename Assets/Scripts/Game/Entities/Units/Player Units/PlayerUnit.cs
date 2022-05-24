using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerUnit : Unit {
    public override Team Team => Team.DefaultPlayer;
    protected void Awake(){
        properties.curHealthPoint = properties.maxHealthPoint;
        OnAwake();
    }
    protected abstract void OnAwake();
    protected void Start(){
        //UnitManager.current.AddUnit(this);
        OnStart();
    }
    protected abstract void OnStart();
    public Sprite GetSprite() {
        return spriteRenderer.sprite;
    }
    protected void OnDestroy() {
        //UnitManager.current.RemoveUnit(this);
        OnUnitDestroy();
    }
    protected abstract void OnUnitDestroy();
}
