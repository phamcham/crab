using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleCrabUnit : PlayerUnit, IDamagable, ISelectable, ITakeOrder, ISaveObject<BubbleCrabUnitSaveData> {
    public override UnitType type => UnitType.Bubble;
    [Header("Crab settings")]
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    UnitNavMovement movement;
    UnitTaskShooting shootable;
    UIE_BubbleCrabUnitControl uiControl;
    UnitTaskManager taskManager;
    FlashOnImpactEffect impactEffect;


    protected override void OnAwake() {
        //properties = new UnitProperties(Team.DefaultPlayer, 100, 10, 6);
        //unitTasks = GetComponents<UnitTask>();
        movement = GetComponent<UnitNavMovement>();
        shootable = GetComponent<UnitTaskShooting>();
        taskManager = GetComponent<UnitTaskManager>();

        if (!base.spriteRenderer.TryGetComponent(out impactEffect)) {
            impactEffect = base.spriteRenderer.gameObject.AddComponent<FlashOnImpactEffect>();
        }
    }
    protected override void OnStart() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_BubbleCrabUnitControl>(this);
        uiControl.SetUnitShootable(shootable);
        uiControl.Hide();

        selectorObj.SetActive(false);

        healthBar.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBar.Hide();
        //print("false roi ma");
        UnitManager.current.BubbleCrabUnits.Add(this);
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
        uiControl.Hide();
    }
    // event unitselectable
    public void OnShowControlUI(bool isShow) {
        if (isShow) uiControl.Show();
        else uiControl.Hide();
    }

    protected override void OnUnitDestroy() {
        SelectionOneUIControlManager.current.RemoveUIControl(this);
        UnitManager.current.BubbleCrabUnits.Remove(this);
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
            if (hit.collider.TryGetComponent(out EnemyUnit enemyUnit)) {
                shootable.SetFollowEnemy(enemyUnit);
                //shootable.StartDoTask();
                //print("kill :" + enemyUnit.name);
                return;
            }
        }
        movement.MoveToPosition(position);
    }

    public BubbleCrabUnitSaveData GetSaveObjectData() {
        return new BubbleCrabUnitSaveData() {
            unit = new UnitSaveData() {
                maxHealthPoint = properties.maxHealthPoint,
                curHealthPoint = properties.curHealthPoint,
                moveSpeed = properties.moveSpeed,
                damage = properties.damage,
                position = new SaveSystemExtension.Vector2(transform.position)
            },
            shooting = shootable.GetSaveObjectData(),
            movement = movement.GetSaveObjectData()
        };
    }
}

[System.Serializable]
public struct BubbleCrabUnitSaveData {
    public UnitSaveData unit;
    public TaskShootingSaveData shooting;
    public MovementSaveData movement;
}
