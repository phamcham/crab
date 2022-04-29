using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBuilding : Building
{
    [SerializeField] GameObject selectorObj;
    public override void OnBuildingPlaced() {
        
    }
    private void Start() {
        selectorObj.SetActive(false);
    }
    public override void OnSelected() {
        selectorObj.SetActive(true);
    }

    public override void OnDeselected() {
        selectorObj.SetActive(false);
    }

    public override void ShowControlUI(bool active)
    {
        
    }
}
