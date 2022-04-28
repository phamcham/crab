using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CrabUnit : Unit  {
    
    [Header("Crab settings")]
    [SerializeField] Transform spriteTrans;
    [SerializeField] GameObject selectorObj;
    [SerializeField] UIE_CrabUnitControl uieControlUIPrefab;
    UnitMovement movement;
    UIE_CrabUnitControl uie;

    protected override void OnAwake() {
        Properties = new UnitProperties(Team.DefaultPlayer, 100, 10, 6);
        //unitTasks = GetComponents<UnitTask>();
        movement = GetComponent<UnitMovement>();
    }
    protected override void OnStart() {
        //movement.MoveToPosition(transform.position);
        selectorObj.SetActive(false);
    }
    public override void OnSelected(){
        Transform holder = SelectionOneUIControlManager.current.GetHolder();
        uie = Instantiate(uieControlUIPrefab.gameObject, holder).GetComponent<UIE_CrabUnitControl>();
        //Debug.Assert(uie != null);
        //SelectionOneUIControlManager.current.AddControlUI(uie);
        selectorObj.SetActive(true);
    }
    public override void OnDeselected(){
        if (uie) Destroy(uie.gameObject);
        selectorObj.SetActive(false);
    }
}