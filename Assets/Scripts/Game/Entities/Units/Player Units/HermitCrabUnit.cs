using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermitCrabUnit : PlayerUnit, IDamagable, ISelectable, ITakeOrder, ISaveObject<HermitCrabUnitSaveData> {
    public override UnitType type => UnitType.Hermit;
    [Header("Crab settings")]
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    UnitNavMovement movement;
    
    UnitTaskCloseQuartersCombat taskCombat;
    UIE_HermitCrabUnitControl uiControl;
    UnitTaskManager taskManager;
    FlashOnImpactEffect impactEffect;
    protected override void OnAwake() {
        movement = GetComponent<UnitNavMovement>();
        taskCombat = GetComponent<UnitTaskCloseQuartersCombat>();
        taskManager = GetComponent<UnitTaskManager>();
        if (!base.spriteRenderer.TryGetComponent(out impactEffect)) {
            impactEffect = base.spriteRenderer.gameObject.AddComponent<FlashOnImpactEffect>();
        }
    }
    protected override void OnStart() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_HermitCrabUnitControl>(this);
        uiControl.SetUnit(this);
        uiControl.Hide();

        selectorObj.SetActive(false);

        healthBar.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBar.Hide();

        UnitManager.current.HermitCrabUnits.Add(this);
        UnitManager.current.UpdateUnitAmountUI();
    }
    public void OnSelected(){
        selectorObj.SetActive(true);
        healthBar.Show();
    }
    // event unitselectable
    public void OnDeselected(){
        selectorObj.SetActive(false);
        healthBar.Hide();
    }

    public void OnShowControlUI(bool isShow) {
        if (isShow) uiControl.Show();
        else uiControl.Hide();
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

    protected override void OnUnitDestroy() {
        SelectionOneUIControlManager.current.RemoveUIControl(this);
        UnitManager.current.HermitCrabUnits.Remove(this);
        UnitManager.current.UpdateUnitAmountUI();
    }

    public void OnTakeOrderAtPosition(Vector2 position) {
        taskManager.StopAllTasks();
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector3.forward);
        foreach (RaycastHit2D hit in hits) {
            // ===== Bam vao enemy
            if (hit.collider.TryGetComponent(out EnemyUnit enemyUnit)) {
                taskCombat.SetFollowEnemy(enemyUnit);
                taskCombat.StartDoTask(); // khong can thiet
                return;
            }
        }
        movement.MoveToPosition(position);
    }

    public HermitCrabUnitSaveData GetSaveObjectData() {
        return new HermitCrabUnitSaveData() {
            unit = new UnitSaveData() {
                maxHealthPoint = properties.maxHealthPoint,
                curHealthPoint = properties.curHealthPoint,
                moveSpeed = properties.moveSpeed,
                damage = properties.damage,
                position = new SaveSystemExtension.Vector2(transform.position)                
            },
            movement = movement.GetSaveObjectData()
        };
    }
}

[System.Serializable]
public struct HermitCrabUnitSaveData {
    public UnitSaveData unit;
    public MovementSaveData movement;
}