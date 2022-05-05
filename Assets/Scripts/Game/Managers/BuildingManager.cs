using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager current { get; private set; }
    [SerializeField] UI_Building ui;
    [SerializeField] HeadquarterBuilding headquarterBuilding;
    [SerializeField] List<Building> otherBuildingPrefabs;
    private void Awake() {
        current = this;
    }

    private void Start() {
        ui.AddOnceHeadquarterBuildingUI(headquarterBuilding);
    }

    public void RemoveHeadquarterUIAndAddOthersBuildingUI() {
        ui.RemoveOnceHeadquarterBuildingUI();
        foreach (Building building in otherBuildingPrefabs){
            ui.AddBuildingUI(building);
        }
    }
}
