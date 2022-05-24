using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager current { get; private set; }
    [SerializeField] UI_Building ui;
    [SerializeField] HeadquarterBuilding headquarterBuildingPrefab;
    [SerializeField] List<Building> otherBuildingPrefabs;
    HeadquarterBuilding currentHeadquarter;
    public List<HouseBuilding> HouseBuildings { get; set; } = new List<HouseBuilding>();
    public List<SandWallBuilding> SandWallBuildings { get; set; } = new List<SandWallBuilding>();
    public List<LawnSprinklerBuilding> LawnSprinklerBuildings { get; set; } = new List<LawnSprinklerBuilding>();
    private void Awake() {
        current = this;
    }

    private void Start() {
        ui.AddOnceHeadquarterBuildingUI(headquarterBuildingPrefab);
    }

    public void RemoveHeadquarterUIAndAddOthersBuildingUI() {
        ui.RemoveOnceHeadquarterBuildingUI();
        foreach (Building building in otherBuildingPrefabs){
            ui.AddBuildingUI(building);
        }
    }
    public void HeadquarterStartGameplay(HeadquarterBuilding headquarter){
        currentHeadquarter = headquarter;
        BuildingManager.current.RemoveHeadquarterUIAndAddOthersBuildingUI();
    }
    public HeadquarterBuilding GetHeadquarterBuilding(){
        return currentHeadquarter;
    }
}
