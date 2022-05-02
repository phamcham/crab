using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilding : Building, IDamagable {
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    public OwnProperties ownProperties;
    BuildingSelectable selectable;
    public override Team Team => Team.DefaultPlayer;
    private void Awake() {
        selectable = GetComponent<BuildingSelectable>();

        selectable.OnSelected = OnSelectedHandle;
        selectable.OnDeselected = OnDeselectedHandle;
        selectable.OnShowControlUI = OnShowControlUIHandle;
    }
    private void Start() {
        selectorObj.SetActive(false);
        healthBar.Hide();
    }
    private void OnSelectedHandle() {
        selectorObj.SetActive(true);
    }

    private void OnDeselectedHandle() {
        selectorObj.SetActive(false);
    }

    private void OnShowControlUIHandle(bool active){
        UIE_HouseBuildingControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_HouseBuildingControl>(this);
        uie.Setup(this);
        uie.gameObject.SetActive(active);
    }

    public override void OnBuildingPlaced() {
        UnitManager.current.ChangeHouseCapacity(ownProperties.capacity);
    }

    public void TakeDamage(int damage)
    {
        int curHeath = properties.curHealthPoint;
        int maxHeath = properties.maxHealthPoint;
        //print("cuerh: " + curHeath + "/" + maxHeath);
        curHeath -= damage;
        if (curHeath < 0) curHeath = 0;
        properties.curHealthPoint = curHeath;

        healthBar.SetSize(1.0f * curHeath / maxHeath);
    }

    protected override void OnDestroyBuilding()
    {
        SelectionOneUIControlManager.current.RemoveUIControl(this);
        UnitManager.current.ChangeHouseCapacity(-ownProperties.capacity);
    }

    [System.Serializable]
    public struct OwnProperties {
        public int capacity;
    }
}
