using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterBuilding : Building, IDamagable, ISelectable {
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    public OwnProperties ownProperties;
    BuildingStorage storage;
    private float time;
    private bool isStartSpawned;
    private int circleIndex;
    bool isPrevSpawnSuccess = true;
    bool isContinue = true;
    bool enoughHouseForCrabs = false;
    public override Team Team => Team.DefaultPlayer;
    UIE_HeadquarterBuildingControl uiControl;
    private void Awake() {
        storage = GetComponent<BuildingStorage>();
    }
    private void Start() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_HeadquarterBuildingControl>(this);
        uiControl.SetBuilding(this);
        uiControl.Hide();

        selectorObj.SetActive(false);
        
        healthBar.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBar.Hide();
    }
    public override void OnBuildingPlaced() {
        GameController.current.HeadquarterStartGameplay(this);
        UnitManager.current.ChangeHouseCapacity(ownProperties.houseCapacity);
        isStartSpawned = true;
        time = ownProperties.productionInterval;
        storage.CanStoraged = true;

        if (UnitManager.current.unitCount >= UnitManager.current.houseCapacity) {
            enoughHouseForCrabs = true;
        }
        else {
            enoughHouseForCrabs = false;
        }
    }
    public void OnSelected() {
        selectorObj.SetActive(true);
        healthBar.Show();
    }
    public void OnDeselected() {
        selectorObj.SetActive(false);
        healthBar.Hide();
    }
    private bool SpawnNewCrab(){
        // TODO: kiem tra co de phai con cua nao khong????

        for (int i = 0; i < 30; i++) {
            Vector2 randomPosition = (Vector2)transform.position + Random.Range(3, 6) * new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f)).normalized;
            bool isWalkable = NavMeshMap.current.IsWalkable(randomPosition);
            if (isWalkable) {
                Unit unit = UnitManager.current.Create<GatheringCrabUnit>();
                unit.transform.position = randomPosition;
                if (unit.TryGetComponent(out UnitNavMovement movement)){
                    //movement.Move(randomPosition);
                }

                return true;
            }
        }
        return false;
    }
    public void PauseProduction() {
        isContinue = false;
    }
    public void ContinueProduction() {
        isContinue = true;
    }
    private bool HasUnitInPosition(Vector2 position, float size) {
        Collider2D[] cols = Physics2D.OverlapBoxAll(position, Vector2.one * size, 0);
        //Debug.Log(cols.Length);
        foreach (Collider2D col in cols) {
            //Debug.Log(col.name);
            if (col.TryGetComponent(out Unit unit)) {
                return true;
            }
        }
        return false;
    }
    private void Update() {
        if (isContinue && isStartSpawned){
            if (time <= 0){
                if (UnitManager.current.unitCount >= UnitManager.current.houseCapacity) {
                    if (enoughHouseForCrabs) {
                        // thong bao k du nha
                        enoughHouseForCrabs = false;
                        UIE_HeadquarterBuildingControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_HeadquarterBuildingControl>(this);
                        uie.EnoughCapacityForCrab(false);
                    }
                }
                else {
                    if (!enoughHouseForCrabs) {
                        enoughHouseForCrabs = true;
                        UIE_HeadquarterBuildingControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_HeadquarterBuildingControl>(this);
                        uie.EnoughCapacityForCrab(true);
                    }
                    isPrevSpawnSuccess = SpawnNewCrab();
                    if (!isPrevSpawnSuccess) {
                        // TODO: thong bao khong the spawn vi het cho de spawn roi
                    }
                    else {
                        time = ownProperties.productionInterval;
                    }
                }
            }
            else {
                if (isPrevSpawnSuccess){
                    ownProperties.curProductionPercent = 100 - Mathf.RoundToInt(100 * time / ownProperties.productionInterval);
                }
                time -= Time.deltaTime;
            }
        }
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

    [System.Serializable]
    public struct OwnProperties {
        public int productionInterval;
        [HideInInspector]
        public int curProductionPercent;
        public int houseCapacity;
    }
}
