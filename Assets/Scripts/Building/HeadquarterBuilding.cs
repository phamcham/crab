using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterBuilding : Building {
    public override void OnBuildingPlaced(){
        GameController.current.HeadquarterStartGameplay(this);
        //UI_Building.current.HeadquarterPlaced();
    }
}
