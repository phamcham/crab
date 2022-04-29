using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterBuilding : Building {
    [SerializeField] GameObject selectorObj;
    //UIE_HeadquarterBuildingControl uie;

    private void Start() {
        selectorObj.SetActive(false);
    }
    public override void OnBuildingPlaced(){
        GameController.current.HeadquarterStartGameplay(this);
    }
    public override void OnSelected() {
        selectorObj.SetActive(true);
        // setting uie
    }

    public override void OnDeselected() {
        selectorObj.SetActive(false);
    }

    // ly do khong de vao select vi minh chi select 1 cai thoi THANG NGUUUU
    public override void ShowControlUI(bool active){
        // if (uie == null) {
        //     Transform holder = SelectionOneUIControlManager.current.GetHolder();
        //     uie = Instantiate(headquarterControlUIPrefab.gameObject, holder).GetComponent<UIE_HeadquarterBuildingControl>();
        // }
        UIE_HeadquarterBuildingControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_HeadquarterBuildingControl>(this);
        uie.gameObject.SetActive(active);
    }
    private void OnDestroy() {
        // if (uie && uie.gameObject) {
        //     Destroy(uie.gameObject);
        // }
        SelectionOneUIControlManager.current.RemoveUIControl(this);
    }
}
