using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterBuilding : Building {
    [SerializeField] GameObject selectorObj;
    [SerializeField] UIE_HeadquarterBuildingControl headquarterControlUIPrefab;
    UIE_HeadquarterBuildingControl uie;

    private void Start() {
        selectorObj.SetActive(false);
    }
    public override void OnBuildingPlaced(){
        GameController.current.HeadquarterStartGameplay(this);
    }
    public override void OnSelected() {
        Transform holder = SelectionOneUIControlManager.current.GetHolder();
        uie = Instantiate(headquarterControlUIPrefab.gameObject, holder).GetComponent<UIE_HeadquarterBuildingControl>();
        //SelectionOneUIControlManager.current.AddControlUI(uie);
        selectorObj.SetActive(true);
        // setting uie
    }

    public override void OnDeselected() {
        if (uie) Destroy(uie.gameObject);
        selectorObj.SetActive(false);
    }
}
