using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GatheringCrabUnit : PlayerUnit, IDamagable, ISelectable, ITakeOrder, ISaveObject<GatheringCrabUnitSaveData> {
    public override UnitType type => UnitType.Gathering;
    
    [Header("Crab settings")]
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    UnitNavMovement movement;
    UnitTaskGathering taskGathering;
    UnitTaskCloseQuartersCombat taskCombat;
    UIE_GatheringCrabUnitControl uiControl;
    UnitTaskManager taskManager;
    FlashOnImpactEffect impactEffect;
    protected override void OnAwake() {
        movement = GetComponent<UnitNavMovement>();
        taskGathering = GetComponent<UnitTaskGathering>();
        taskCombat = GetComponent<UnitTaskCloseQuartersCombat>();
        taskManager = GetComponent<UnitTaskManager>();

        if (!base.spriteRenderer.TryGetComponent(out impactEffect)) {
            impactEffect = base.spriteRenderer.gameObject.AddComponent<FlashOnImpactEffect>();
        }
    }
    protected override void OnStart() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_GatheringCrabUnitControl>(this);
        uiControl.SetUnit(this);
        uiControl.Hide();

        selectorObj.SetActive(false);

        healthBar.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBar.Hide();
        //print("false roi ma");
        UnitManager.current.GatheringCrabUnits.Add(this);
        UnitManager.current.UpdateUnitAmountUI();
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
        UnitManager.current.GatheringCrabUnits.Remove(this);
        UnitManager.current.UpdateUnitAmountUI();
    }

    public void TakeDamage(int damage){
        int curHeath = properties.curHealthPoint;
        int maxHeath = properties.maxHealthPoint;
        //print("cuerh: " + curHeath + "/" + maxHeath);
        curHeath -= damage;
        if (curHeath < 0) curHeath = 0;
        properties.curHealthPoint = curHeath;

        healthBar.SetSize(1.0f * curHeath / maxHeath);

        impactEffect.Impact();
        if (curHeath == 0) {
            Destroy(gameObject);
        }
    }

    public void OnTakeOrderAtPosition(Vector2 position) {
        taskManager.StopAllTasks();

        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector3.forward);
        foreach (RaycastHit2D hit in hits) {
            // ===== Bam vao building
            if (hit.collider.TryGetComponent(out HeadquarterBuilding storage)) {
                taskGathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToStorage);
                taskGathering.StartDoTask();
                return;
            }
            // ===== Bam vao resource
            if (hit.collider.TryGetComponent(out Resource resource)) {
                taskGathering.SetResource(resource);
                taskGathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToResource);
                taskGathering.StartDoTask();
                return;
            }
            // ===== Bam vao enemy
            if (hit.collider.TryGetComponent(out EnemyUnit enemyUnit)) {
                taskCombat.SetFollowEnemy(enemyUnit);
                taskCombat.StartDoTask(); // khong can thiet
                return;
            }
        }
        movement.MoveToPosition(position);
    }

    public GatheringCrabUnitSaveData GetSaveObjectData() {
        return new GatheringCrabUnitSaveData() {
            unit = new UnitSaveData() {
                maxHealthPoint = properties.maxHealthPoint,
                curHealthPoint = properties.curHealthPoint,
                moveSpeed = properties.moveSpeed,
                damage = properties.damage,
                position = new SaveSystemExtension.Vector2(transform.position)                
            },
            gathering = this.taskGathering.GetSaveObjectData(),
            closeQuartersCombat = this.taskCombat.GetSaveObjectData(),
            movement = this.movement.GetSaveObjectData()
        };
    }
}

[System.Serializable]
public struct GatheringCrabUnitSaveData {
    public UnitSaveData unit;
    public TaskGatheringSaveData gathering;
    public TaskCloseQuartersCombatSaveData closeQuartersCombat;
    public MovementSaveData movement;
}