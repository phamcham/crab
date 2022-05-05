using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBuilding : Building, ISelectable
{
    [SerializeField] GameObject selectorObj;
    public override Team Team => Team.DefaultPlayer;

    public override void OnBuildingPlaced() {
        
    }
    private void Awake() {
    }
    private void Start() {
        selectorObj.SetActive(false);
        //healthBar.Hide();
    }
    public void OnSelected() {
        selectorObj.SetActive(true);
    }

    public void OnDeselected() {
        selectorObj.SetActive(false);
    }

    public void OnShowControlUI(bool isShow) {
        
    }

    protected override void OnDestroyBuilding()
    {
        throw new System.NotImplementedException();
    }

    public void OnGiveOrder(Vector2 position) {
        OnDeselected();
    }
}
