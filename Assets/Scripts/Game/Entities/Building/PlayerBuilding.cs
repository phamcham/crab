using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBuilding : Building {
    public override Team Team => Team.DefaultPlayer;
}
