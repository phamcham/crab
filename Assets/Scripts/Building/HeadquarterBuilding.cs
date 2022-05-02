using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterBuilding : Building, IDamagable {
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    public OwnProperties ownProperties;
    BuildingSelectable selectable;
    BuildingStorage storage;
    private float time;
    private bool isStartSpawned;
    private List<Vector2Int> circles = new List<Vector2Int>();
    private Vector2Int center;
    private int circleIndex;
    bool isPrevSpawnSuccess = true;
    bool isContinue = true;
    bool enoughHouseForCrabs = false;

    public override Team Team => Team.DefaultPlayer;
    private void Awake() {
        storage = GetComponent<BuildingStorage>();
        selectable = GetComponent<BuildingSelectable>();

        selectable.OnSelected = OnSelectedHandle;
        selectable.OnDeselected = OnDeselectedHandle;
        selectable.OnShowControlUI = OnShowControlUIHandle;
    }
    private void Start() {
        selectorObj.SetActive(false);
        healthBar.Hide();
    }
    public override void OnBuildingPlaced(){
        GameController.current.HeadquarterStartGameplay(this);
        isStartSpawned = true;
        center = AStarGrid.current.WorldPositionToGridPosition(transform.position);
        circles = new List<Vector2Int>();
        circles.AddRange(BresenhamsCircle.CircleBres(center, 2));
        circles.AddRange(BresenhamsCircle.CircleBres(center, 3));
        circles = new List<Vector2Int>(new HashSet<Vector2Int>(circles));
        time = ownProperties.productionInterval;
        storage.CanStoraged = true;

        if (UnitManager.current.unitCount >= UnitManager.current.houseCapacity) {
            enoughHouseForCrabs = true;
        }
        else {
            enoughHouseForCrabs = false;
        }
    }
    private void OnSelectedHandle() {
        selectorObj.SetActive(true);
        // setting uie
    }
    private void OnDeselectedHandle() {
        selectorObj.SetActive(false);
    }
    private bool SpawnNewCrab(){
        // TODO: kiem tra co de phai con cua nao khong????

        for (int i = 1; i < circles.Count; i++){
            int nextIndex = (i + circleIndex) % circles.Count;
            Vector2Int nextPosition = circles[nextIndex];
            AStarNode node = AStarGrid.current.NodeFromGridPoint(nextPosition);
            if (node == null) continue;

            bool isWalkable = node.walkable;
            Vector3 wpos = node.worldPosition;
            // TODO: kiem tra co con cua nao dang dung o do khong
            bool hasUnit = HasUnitInPosition(wpos, 0.4f);
            if (isWalkable && !hasUnit) {
                Unit unit = UnitManager.current.Create<GatheringCrabUnit>();
                unit.transform.position = transform.position;
                if (unit.TryGetComponent(out UnitMovement movement)){
                    movement.MoveToPosition(wpos);
                    //unit.transform.position = wpos;
                }
                circleIndex = nextIndex;
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
    private void OnDrawGizmos() {
        if (circles != null) {
            Color color = Color.white;
            color.a = 0.3f;
            Gizmos.color = color;
            foreach (var pos in circles){
                AStarNode node = AStarGrid.current.NodeFromGridPoint(pos);
                Vector3 wpos = node.worldPosition;
                Gizmos.DrawCube(wpos, Vector3.one);
            }
        }
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
                    time = ownProperties.productionInterval;
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

    // ly do khong de vao select vi minh chi select 1 cai thoi THANG NGUUUU
    private void OnShowControlUIHandle(bool active){
        UIE_HeadquarterBuildingControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_HeadquarterBuildingControl>(this);
        uie.Setup(this);
        uie.gameObject.SetActive(active);
    }

    public void TakeDamage(int damage) {
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
    }

    [System.Serializable]
    public struct OwnProperties {
        public int productionInterval;
        [HideInInspector]
        public int curProductionPercent;
    }
}
