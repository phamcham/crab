using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuildingManager : MonoBehaviour
{
    public static UIBuildingManager current { get; private set; }
    [SerializeField] GameObject headquarter;
    [SerializeField] GameObject building;

    private void Awake() {
        current = this;
    }
    private void Start() {
        headquarter.SetActive(true);
        building.SetActive(false);
    }
    public void HeadquarterPlaced(){
        headquarter.SetActive(false);
        building.SetActive(true);
    }
}