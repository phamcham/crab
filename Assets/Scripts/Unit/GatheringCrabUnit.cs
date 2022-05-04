using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GatheringCrabUnit : PlayerUnit, IDamagable, ISelectable {
    
    [Header("Crab settings")]
    [SerializeField] HealthBar healthBar;
    [SerializeField] Transform spriteTrans;
    [SerializeField] GameObject selectorObj;
    UnitMovement movement;
    UnitTaskGathering gathering;
    UnitCloseQuartersCombat combat;
    UIE_CrabUnitControl uiControl;
    protected override void OnAwake() {
        movement = GetComponent<UnitMovement>();
        gathering = GetComponent<UnitTaskGathering>();
        combat = GetComponent<UnitCloseQuartersCombat>();
    }
    protected override void OnStart() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_CrabUnitControl>(this);
        uiControl.SetUnit(this);
        uiControl.Hide();

        selectorObj.SetActive(false);

        healthBar.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBar.Hide();
        //print("false roi ma");
    }

    // event unitselectable
    public void OnSelected(){
        selectorObj.SetActive(true);
        healthBar.Show();
    }
    // event unitselectable
    public void OnDeselected(){
        selectorObj.SetActive(false);
        healthBar.Hide();
    }
    // event unitselectable
    public void OnShowControlUI(bool isShow) {
        if (isShow) uiControl.Show();
        else uiControl.Hide();
    }

    protected override void OnUnitDestroy() {
        SelectionOneUIControlManager.current.RemoveUIControl(this);
    }

    public void TakeDamage(int damage){
        int curHeath = properties.curHealthPoint;
        int maxHeath = properties.maxHealthPoint;
        //print("cuerh: " + curHeath + "/" + maxHeath);
        curHeath -= damage;
        if (curHeath < 0) curHeath = 0;
        properties.curHealthPoint = curHeath;

        healthBar.SetSize(1.0f * curHeath / maxHeath);

        if (curHeath == 0) {
            Destroy(gameObject);
        }
    }

    public void OnGiveOrder(Vector2 position) {
        gathering.EndDoTask();

        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector3.forward);
        foreach (RaycastHit2D hit in hits) {
            // ===== Bam vao building
            if (hit.collider.TryGetComponent(out BuildingStorage storage)) {
                gathering.SetBuildingStorage(storage);
                gathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToStorage);
                gathering.StartDoTask();
                return;
            }
            // ===== Bam vao resource
            if (hit.collider.TryGetComponent(out Resource resource)) {
                gathering.SetResource(resource);
                gathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToResource);
                gathering.StartDoTask();
                return;
            }
            // ===== Bam vao enemy
            if (hit.collider.TryGetComponent(out EnemyUnit enemyUnit)) {
                combat.SetFollowEnemy(enemyUnit);
            }
        }
        movement.MoveToPosition(position);
    }
}