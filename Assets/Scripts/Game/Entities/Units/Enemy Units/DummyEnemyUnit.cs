using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyUnit : EnemyUnit {
    public override UnitType type => throw new System.NotImplementedException();

    protected override void OnAwake()
    {
    }

    protected override void OnStart()
    {
    }

    protected override void OnUnitDestroy()
    {
    }
}
