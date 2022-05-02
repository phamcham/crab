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
            List<Building> buildings = GetBuildingDamagables();
            Vector2 direction = Vector2.zero;
            if (buildings != null && buildings.Count > 0) {
                direction = (buildings[0].transform.position - transform.position).normalized;
            }
            else {
                List<Unit> units = GetUnitDamagables();
                if (units != null && units.Count > 0) {
                    direction = (units[0].transform.position - transform.position).normalized;
                }
            }
            
            if (direction != Vector2.zero) {
                Bullet bullet = BulletManager.GetBulletPooled();
                bullet.transform.position = transform.position;
                float lifeTime = 1.0f * attackRadius / bulletSpeed;
                bullet.Shoot(new BulletProperties(BaseEnemyUnit.Team, bulletSpeed, BaseEnemyUnit.properties.damage, direction), lifeTime);
            }
        }
    }

    private List<Unit> GetUnitDamagables() {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, attackRadius);
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
    private List<Building> GetBuildingDamagables() {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, attackRadius);
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
}
