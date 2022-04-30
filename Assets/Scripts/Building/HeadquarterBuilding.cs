using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterBuilding : Building {
    public OwnProperties ownProperties;
    [SerializeField] GameObject selectorObj;
    BuildingSelectable selectable;
    private float time;
    private bool isStartSpawned;
    private List<Vector2Int> circles = new List<Vector2Int>();
    private Vector2Int center;
    private int circleIndex;
    bool isPrevSpawnSuccess = true;
    bool isContinue = true;

    public override Team Team => Team.DefaultPlayer;
    private void Awake() {
        selectable = GetComponent<BuildingSelectable>();

        selectable.OnSelected = OnSelectedHandle;
        selectable.OnDeselected = OnDeselectedHandle;
        selectable.OnShowControlUI = OnShowControlUIHandle;
    }
    private void Start() {
        selectorObj.SetActive(false);
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
                Unit unit = UnitManager.current.Create<CrabUnit>();
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
                isPrevSpawnSuccess = SpawnNewCrab();
                if (!isPrevSpawnSuccess) {
                    // TODO: thong bao khong the spawn vi het cho roi
                }
                time = ownProperties.productionInterval;
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
        // if (uie == null) {
        //     Transform holder = SelectionOneUIControlManager.current.GetHolder();
        //     uie = Instantiate(headquarterControlUIPrefab.gameObject, holder).GetComponent<UIE_HeadquarterBuildingControl>();
        // }
        UIE_HeadquarterBuildingControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_HeadquarterBuildingControl>(this);
        uie.Setup(this);
        uie.gameObject.SetActive(active);
    }
    private void OnDestroy() {
        // if (uie && uie.gameObject) {
        //     Destroy(uie.gameObject);
        // }
        SelectionOneUIControlManager.current.RemoveUIControl(this);
    }
    [System.Serializable]
    public struct OwnProperties {
        public int productionInterval;
        [HideInInspector]
        public int curProductionPercent;
    }
}
