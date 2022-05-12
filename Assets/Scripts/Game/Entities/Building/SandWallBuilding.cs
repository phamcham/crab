using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWallBuilding : Building, ISelectable, IDamagable
{
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    public override Team Team => Team.DefaultPlayer;
    UIE_SandWallBuildingControl uiControl;

    public override void OnBuildingPlaced() {
        
    }
    private void Start() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_SandWallBuildingControl>(this);
        uiControl.SetBuilding(this);
        uiControl.Hide();

        selectorObj.SetActive(false);

        healthBar.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBar.Hide();
    }
    public void OnSelected() {
        selectorObj.SetActive(true);
        healthBar.Show();
    }

    public void OnDeselected() {
        selectorObj.SetActive(false);
        healthBar.Hide();
    }

    public void OnShowControlUI(bool isShow) {
        if (isShow) uiControl.Show();
        else uiControl.Hide();
    }

    protected override void OnDestroyBuilding() {
        SelectionOneUIControlManager.current.RemoveUIControl(this);
    }

    public void OnGiveOrder(Vector2 position) {
        OnDeselected();
    }

    public void TakeDamage(int damage) {
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
}
