using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyUnitShootable : MonoBehaviour {
    public int attackRadius;
    public float reloadingTime;
    public int bulletSpeed;
    private float curReloadingTime = 0;
    const float checkInterval = 2f;
    float curCheck = 0;
    Vector2 direction = Vector2.zero;
    public EnemyUnit BaseEnemyUnit { get; private set; }
    
    private void Awake() {
        BaseEnemyUnit = GetComponent<EnemyUnit>();
    }
    private void Update() {
        if (curCheck < checkInterval) {
            curCheck += Time.deltaTime;
        }
        else {
            curCheck = 0;
            List<Building> buildings = GetBuildingDamagables(attackRadius);
            if (buildings != null && buildings.Count > 0) {
                Building building = GetNearestBuilding(buildings);
                direction = (building.transform.position - transform.position).normalized;
            }
            else {
                List<Unit> units = GetUnitDamagables(attackRadius);
                if (units != null && units.Count > 0) {
                    Unit unit = GetNearestUnit(units);
                    direction = (unit.transform.position - transform.position).normalized;
                }
                else {
                    direction = Vector2.zero;
                }
            }
        }

        if (curReloadingTime <= 0) {
            //Debug.Log(direction);
            curReloadingTime = reloadingTime;
            if (direction != Vector2.zero) {
                Bullet bullet = BulletManager.GetBulletPooled();
                bullet.transform.position = transform.position;
                float lifeTime = 1.0f * attackRadius / bulletSpeed;
                bullet.Shoot(new BulletProperties(BaseEnemyUnit.Team, bulletSpeed, BaseEnemyUnit.properties.damage, direction), lifeTime);
            }
        }

        curReloadingTime = Mathf.Max(0, curReloadingTime - Time.deltaTime);
    }

    private List<Unit> GetUnitDamagables(float radius) {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        List<Unit> list = new List<Unit>();
        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out Unit unit)) {
                if (unit.TryGetComponent(out IDamagable damagable)) {
                    if (damagable.Team != BaseEnemyUnit.Team){
                        list.Add(unit);
                    }
                }
            }
        }
        return list;
    }
    private Unit GetNearestUnit(List<Unit> list) {
        float minDistance = float.MaxValue;
        Unit res = null;
        foreach (Unit unit in list) {
            float distance = Vector2.Distance(unit.transform.position, transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                res = unit;
            }
        }
        return res;
    }
    private List<Building> GetBuildingDamagables(float radius) {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        List<Building> list = new List<Building>();
        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out Building unit)) {
                if (unit.TryGetComponent(out IDamagable damagable)) {
                    if (damagable.Team != BaseEnemyUnit.Team){
                        list.Add(unit);
                    }
                }
            }
        }
        return list;
    }

    private Building GetNearestBuilding(List<Building> list) {
        float minDistance = float.MaxValue;
        Building res = null;
        foreach (Building building in list) {
            float distance = Vector2.Distance(building.transform.position, transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                res = building;
            }
        }
        return res;
    }
}
