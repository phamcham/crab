using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBuilding : Building {
    public override Team Team => Team.DefaultEnemy;
}
