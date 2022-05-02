using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBuilding : Building
{
    [SerializeField] GameObject selectorObj;
    BuildingSelectable selectable;
    public override Team Team => Team.DefaultPlayer;

    public override void OnBuildingPlaced() {
        
    }
    private void Awake() {
        selectable = GetComponent<BuildingSelectable>();

        selectable.OnSelected = OnSelected;
        selectable.OnDeselected = OnDeselected;
        selectable.OnShowControlUI = ShowControlUI;
    }
    private void Start() {
        selectorObj.SetActive(false);
        //healthBar.Hide();
    }
    private void OnSelected() {
        selectorObj.SetActive(true);
    }

    private void OnDeselected() {
        selectorObj.SetActive(false);
    }

    private void ShowControlUI(bool active)
    {
        
    }

    protected override void OnDestroyBuilding()
    {
        throw new System.NotImplementedException();
    }
}
