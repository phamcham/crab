using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleCrabUnit : Unit, IDamagable {
    [Header("Crab settings")]
    [SerializeField] HealthBar healthBar;
    [SerializeField] Transform spriteTrans;
    [SerializeField] GameObject selectorObj;
    UnitMovement movement;
    UnitSelectable selectable;
    UnitShootable shootable;
    public override Team Team => Team.DefaultPlayer;
    //UIE_CrabUnitControl uie;

    protected override void OnAwake() {
        //properties = new UnitProperties(Team.DefaultPlayer, 100, 10, 6);
        //unitTasks = GetComponents<UnitTask>();
        movement = GetComponent<UnitMovement>();
        shootable = GetComponent<UnitShootable>();
        selectable = GetComponent<UnitSelectable>();

        selectable.OnSelected = OnSelectedHandle;
        selectable.OnDeselected = OnDeselectedHandle;
        selectable.OnShowControlUI = OnShowControlUIHandle;
    }
    protected override void OnStart() {
        //movement.MoveToPosition(transform.position);
        selectorObj.SetActive(false);
        healthBar.Hide();
        //print("false roi ma");
    }

    // event unitselectable
    public void OnSelectedHandle(){
        selectorObj.SetActive(true);
        healthBar.Show();
    }
    // event unitselectable
    public void OnDeselectedHandle(){
        selectorObj.SetActive(false);
        healthBar.Hide();
    }
    // event unitselectable
    public void OnShowControlUIHandle(bool active) {
        UIE_BubbleCrabUnitControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_BubbleCrabUnitControl>(this);
        uie.Setup(shootable);
        uie.gameObject.SetActive(active);
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
    }
}
