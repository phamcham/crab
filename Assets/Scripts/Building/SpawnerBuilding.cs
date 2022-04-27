using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBuilding : Building {
    float interval = 3f;
    float time;
    bool isStartSpawned;
    public override void OnBuildingPlaced() {
        isStartSpawned = true;
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
        Vector2Int center = AStarGrid.current.NodeFromWorldPoint(transform.position).gridIndex;
        for (int i = 1; i <= 4; i++){
            List<Vector2Int> list = BresenhamsCircle.CircleBres(center, i);
            bool ok = false;
            foreach (Vector2Int pos in list) {
                AStarNode node = AStarGrid.current.NodeFromGridPoint(pos);
                bool isWalkable = node.walkable;
                Vector3 wpos = node.worldPosition;
                // TODO: kiem tra co con cua nao dang dung o do khong
                if (isWalkable) {
                    Unit unit = UnitManager.current.Create(UnitType.Crab);
                    unit.transform.position = transform.position;
                    if (unit.TryGetComponent(out UnitMovement movement)){
                        movement.MoveToPosition(wpos);
                    }
                    ok = true;
                    break;
                }
            }
            if (ok) break;
        }
    }
}
