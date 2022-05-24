using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HeadquarterBuilding : PlayerBuilding, IDamagable, ISelectable, ISaveObject<HeadquarterBuildingSaveData> {
    public override BuildingType type => BuildingType.Headquarter;
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    public OwnProperties ownProperties;
    UIE_HeadquarterBuildingControl uiControl;
    Queue<Unit> productionUnits = new Queue<Unit>();

    private void Start() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_HeadquarterBuildingControl>(this);
        uiControl.SetBuilding(this);
        uiControl.Hide();

        selectorObj.SetActive(false);
        
        healthBar.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBar.Hide();
    }
    public override void OnBuildingPlaced() {
        BuildingManager.current.HeadquarterStartGameplay(this);
        UnitManager.current.ChangeHouseCapacity(ownProperties.houseCapacity);
    }
    public void OnSelected() {
        selectorObj.SetActive(true);
        healthBar.Show();
    }
    public void OnDeselected() {
        selectorObj.SetActive(false);
        healthBar.Hide();
    }

    private void Update() {
        if (productionUnits.Count > 0) {
            ownProperties.curProductionTime += Time.deltaTime;
            ownProperties.curUnitType = productionUnits.Peek().type;

            if (ownProperties.curProductionTime >= ownProperties.productionInterval) {
                // place unit
                Unit unit = productionUnits.Dequeue();
                unit.gameObject.SetActive(true);
                ownProperties.curProductionTime = 0;
            }
        }
    }

    public void AddProductionQueue(UnitType type) {
        //print("add " + typeof(T).Name);
        if (UnitManager.current.UnitCount + productionUnits.Count >= UnitManager.current.houseCapacity) {
            // du nha roi khong spawn nua
            return;
        }

        Unit unit = UnitManager.current.Create(type);
        unit.gameObject.SetActive(false);
        for (int i = 0; i < 30; i++) {
            Vector2 randomPosition = (Vector2)transform.position + Random.Range(3f, 6f) * new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f)).normalized;
            bool isWalkable = NavMeshMap.current.IsWalkable(randomPosition);
            if (isWalkable) {
                unit.transform.position = randomPosition;
                if (unit.TryGetComponent(out UnitNavMovement movement)){
                    //movement.Move(randomPosition);
                }
                break;
            }
        }
        productionUnits.Enqueue(unit);
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
        UnitManager.current.ChangeHouseCapacity(-ownProperties.houseCapacity);
    }

    public void OnGiveOrder(Vector2 position) {
        OnDeselected();
    }

    public void OnShowControlUI(bool isShow) {
        if (isShow) uiControl.Show();
        else uiControl.Hide();
    }

    public HeadquarterBuildingSaveData GetSaveObjectData() {
        return new HeadquarterBuildingSaveData() {
            building = new BuildingSaveData() {
                maxHealthPoint = properties.maxHealthPoint,
                curHealthPoint = properties.curHealthPoint,
                position = new SaveSystemExtension.Vector2(transform.position)
            },

        };
    }

    [System.Serializable]
    public struct OwnProperties {
        // gathering crab
        public int productionInterval;
        // moi lan chi duoc spawn 1 crab duy nhat
        // capacity
        public int houseCapacity;
        [HideInInspector] public float curProductionTime;
        [HideInInspector] public UnitType curUnitType;
    }
}

[System.Serializable]
public struct HeadquarterBuildingSaveData {
    public BuildingSaveData building;
    public int productionInterval;
    public int houseCapacity;
    public float curProductionTime;
}