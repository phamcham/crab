using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterBuilding : Building {
    private void Start() {
        OnBuildingPlaced += OnBuildingPlacedHandle;
    }

    void OnBuildingPlacedHandle(){
        GameController.current.HeadquarterStartGameplay(this);
        UI_Building.current.HeadquarterPlaced();
    }
}
