using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleCrabUnit : PlayerUnit, IDamagable, ISelectable, ITakeOrder {
    [Header("Crab settings")]
    [SerializeField] HealthBar healthBar;
    [SerializeField] Transform spriteTrans;
    [SerializeField] GameObject selectorObj;
    UnitMovement movement;
    UnitTaskShooting shootable;
    UIE_BubbleCrabUnitControl uiControl;
    UnitTaskManager taskManager;
    FlashOnImpactEffect impactEffect;

    protected override void OnAwake() {
        //properties = new UnitProperties(Team.DefaultPlayer, 100, 10, 6);
        //unitTasks = GetComponents<UnitTask>();
        movement = GetComponent<UnitMovement>();
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
                print("kill :" + enemyUnit.name);
                return;
            }
        }
        movement.MoveToPosition(position);
    }
}
