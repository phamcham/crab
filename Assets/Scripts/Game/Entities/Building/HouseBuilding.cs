using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilding : PlayerBuilding, IDamagable, ISelectable, ISaveObject<HouseBuildingSaveData> {
    public override BuildingType type => BuildingType.House;
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    public OwnProperties ownProperties;
    public override Team Team => Team.DefaultPlayer;

    UIE_HouseBuildingControl uiControl;

    private void Start() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_HouseBuildingControl>(this);
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

    public override void OnBuildingPlaced() {
        UnitManager.current.ChangeHouseCapacity(ownProperties.capacity);
        BuildingManager.current.HouseBuildings.Add(this);
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

    protected override void OnDestroyBuilding() {
        SelectionOneUIControlManager.current.RemoveUIControl(this);
        UnitManager.current.ChangeHouseCapacity(-ownProperties.capacity);
        BuildingManager.current.HouseBuildings.Remove(this);
    }

    public void OnGiveOrder(Vector2 position) {
        OnDeselected();
    }
    public void OnShowControlUI(bool isShow) {
        if (isShow) uiControl.Show();
        else uiControl.Hide();
    }

    public HouseBuildingSaveData GetSaveObjectData() {
        return new HouseBuildingSaveData() {
            building = new BuildingSaveData() {
                maxHealthPoint = properties.maxHealthPoint,
                curHealthPoint = properties.curHealthPoint,
                position = new SaveSystemExtension.Vector2(transform.position)
            }
        };
    }

    [System.Serializable]
    public struct OwnProperties {
        public int capacity;
    }
}

[System.Serializable]
public struct HouseBuildingSaveData {
    public BuildingSaveData building;
}