using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyUnit : Unit {
    public override Team Team => Team.DefaultEnemy;
}
