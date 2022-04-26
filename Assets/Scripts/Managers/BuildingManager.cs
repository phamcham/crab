using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager current { get; private set; }
    [SerializeField] UI_Building ui;
    [SerializeField] List<Building> buildingPrefabs;
    private void Awake() {
        current = this;
    }
    private void Start() {
        foreach (Building building in buildingPrefabs){
            ui.AddBuildingUI(building);
        }
    }
}
