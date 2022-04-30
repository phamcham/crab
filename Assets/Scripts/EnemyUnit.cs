using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyUnit : Unit {
    public override Team Team => Team.DefaultEnemy;

    // public override void OnDeselected()
    // {
    //     throw new System.NotImplementedException();
    // }

    // public override void OnSelected()
    // {
    //     throw new System.NotImplementedException();
    // }

    // public override void ShowControlUI(bool active)
    // {
    //     throw new System.NotImplementedException();
    // }

    // protected override void OnAwake()
    // {
    //     throw new System.NotImplementedException();
    // }

    // protected override void OnStart()
    // {
    //     throw new System.NotImplementedException();
    // }

    // protected override void OnUnitDestroy()
    // {
    //     throw new System.NotImplementedException();
    // }
}
