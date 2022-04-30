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
    UnitDamagable damagable;
    UnitSelectable selectable;
    public override Team Team => Team.DefaultPlayer;
    //UIE_CrabUnitControl uie;

    protected override void OnAwake() {
        //properties = new UnitProperties(Team.DefaultPlayer, 100, 10, 6);
        //unitTasks = GetComponents<UnitTask>();
        movement = GetComponent<UnitMovement>();
        damagable = GetComponent<UnitDamagable>();
        selectable = GetComponent<UnitSelectable>();

        selectable.OnSelected = OnSelectedHandle;
        selectable.OnDeselected = OnDeselectedHandle;
        selectable.OnShowControlUI = OnShowControlUIHandle;
    }
    protected override void OnStart() {
        //movement.MoveToPosition(transform.position);
        selectorObj.SetActive(false);
        //print("false roi ma");
    }

    // event unitselectable
    public void OnSelectedHandle(){
        selectorObj.SetActive(true);
        damagable.SetDisplayHealthbar(true);
    }
    // event unitselectable
    public void OnDeselectedHandle(){
        selectorObj.SetActive(false);
        damagable.SetDisplayHealthbar(false);
    }
    // event unitselectable
    public void OnShowControlUIHandle(bool active) {
        UIE_CrabUnitControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_CrabUnitControl>(this);
        uie.Setup(this);
        uie.gameObject.SetActive(active);
    }

    protected override void OnUnitDestroy() {
        SelectionOneUIControlManager.current.RemoveUIControl(this);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}