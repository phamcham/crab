using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBuilding : Building {
    [SerializeField] GameObject selectorObj;
    float interval = 10f;
    float time;
    bool isStartSpawned;
    List<Vector2Int> circles = new List<Vector2Int>();
    Vector2Int center;
    int circleIndex;
    [SerializeField] UIE_SpawnerBuildingControl spawnerControlUIPrefab;
    UIE_SpawnerBuildingControl uie;
    private void Start() {
        selectorObj.SetActive(false);
    }

    public override void OnBuildingPlaced() {
        isStartSpawned = true;
        center = AStarGrid.current.WorldPositionToGridPosition(transform.position);
        circles = new List<Vector2Int>();
        circles.AddRange(BresenhamsCircle.CircleBres(center, 2));
        circles.AddRange(BresenhamsCircle.CircleBres(center, 3));
        circles = new List<Vector2Int>(new HashSet<Vector2Int>(circles));
        time = interval;
    }
    private void Update() {
        if (isStartSpawned){
            if (time <= 0){
                time = interval;
                SpawnNewCrab();
            }
            else{
                time -= Time.deltaTime;
            }
        }
    }
    void SpawnNewCrab(){
        // TODO: kiem tra co de phai con cua nao khong????

        for (int i = 1; i < circles.Count; i++){
            int nextIndex = i + circleIndex;
            Vector2Int nextPosition = circles[nextIndex];
            AStarNode node = AStarGrid.current.NodeFromGridPoint(nextPosition);
            if (node == null) continue;

            bool isWalkable = node.walkable;
            Vector3 wpos = node.worldPosition;
            // TODO: kiem tra co con cua nao dang dung o do khong
            bool hasUnit = HasUnitInPosition(wpos, 0.5f);
            if (isWalkable && !hasUnit) {
                Unit unit = UnitManager.current.Create(UnitType.Crab);
                unit.transform.position = transform.position;
                if (unit.TryGetComponent(out UnitMovement movement)){
                    movement.MoveToPosition(wpos);
                    //unit.transform.position = wpos;
                }
                circleIndex = nextIndex;
                break;
            }
        }
        
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
    public override void OnSelected() {
        Transform holder = SelectionOneUIControlManager.current.GetHolder();
        uie = Instantiate(spawnerControlUIPrefab.gameObject, holder).GetComponent<UIE_SpawnerBuildingControl>();
        //SelectionOneUIControlManager.current.AddControlUI(uie);
        selectorObj.SetActive(true);
    }

    public override void OnDeselected() {
        if (uie) Destroy(uie);
        selectorObj.SetActive(false);
    }
}
