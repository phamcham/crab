using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleFactoryBuilding : Building {
    public override Team Team => Team.DefaultPlayer;

    public override void OnBuildingPlaced() {
        throw new System.NotImplementedException();
    }

    protected override void OnDestroyBuilding() {
        throw new System.NotImplementedException();
    }
}
