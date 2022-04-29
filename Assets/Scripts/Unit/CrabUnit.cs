using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CrabUnit : Unit  {
    
    [Header("Crab settings")]
    [SerializeField] Transform spriteTrans;
    [SerializeField] GameObject selectorObj;
    UnitMovement movement;
    //UIE_CrabUnitControl uie;

    protected override void OnAwake() {
        //properties = new UnitProperties(Team.DefaultPlayer, 100, 10, 6);
        //unitTasks = GetComponents<UnitTask>();
        movement = GetComponent<UnitMovement>();
    }
    protected override void OnStart() {
        //movement.MoveToPosition(transform.position);
        selectorObj.SetActive(false);
    }

    public override void OnSelected(){
        selectorObj.SetActive(true);
    }
    public override void OnDeselected(){
        selectorObj.SetActive(false);
    }
    public override void ShowControlUI(bool active) {
        // if (uie == null) {
        //     Transform holder = SelectionOneUIControlManager.current.GetHolder();
        //     uie = Instantiate(uieControlUIPrefab.gameObject, holder).GetComponent<UIE_CrabUnitControl>();
        // }
        UIE_CrabUnitControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_CrabUnitControl>(this);
        uie.Setup(this);
        uie.gameObject.SetActive(active);
    }
    private void OnDestroy() {
        // if (uie && uie.gameObject){ 
        //     Destroy(uie.gameObject);
        // }
        SelectionOneUIControlManager.current.RemoveUIControl(this);
    }
}