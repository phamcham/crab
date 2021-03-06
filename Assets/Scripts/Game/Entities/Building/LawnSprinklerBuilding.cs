using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LawnSprinklerBuilding : PlayerBuilding, IDamagable, ISelectable, ISaveObject<LawnSpinklerBuildingSaveData> {
    public override BuildingType type => BuildingType.LawnSpinkler;
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    [SerializeField] Transform source; // nong sung
    public OwnProperties ownProperties;
    bool isStartShooting = false;
    const float checkInterval = 1f;
    float checkTime = 0;
    float curReloadingTime = 0;
    Entity targetEnemy;
    UIE_LawnSprinklerBuildingControl uiControl;


    private void Start() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_LawnSprinklerBuildingControl>(this);
        uiControl.SetBuilding(this);
        uiControl.Hide();

        selectorObj.SetActive(false);

        healthBar.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBar.Hide();
    }
    public override void OnBuildingPlaced() {
        isStartShooting = true;
        BuildingManager.current.LawnSprinklerBuildings.Add(this);
    }

    private void Update() {
        if (isStartShooting) {
            if (!targetEnemy) {
                // neu khong tim thay ke dich
                if (checkTime < checkInterval) {
                    checkTime += Time.deltaTime;
                }
                else {
                    checkTime = 0;
                    // check enemy
                    List<Entity> enemies = GetEnemies();
                    if (enemies.Count > 0) {
                        float minDistance = float.MaxValue;
                        foreach (Entity enemy in enemies) {
                            float distance = Vector2.Distance(enemy.transform.position, transform.position);
                            if (!targetEnemy || distance < minDistance) {
                                targetEnemy = enemy;
                                minDistance = distance;
                            }
                        }
                    }
                }
            }
            else {
                float distance = Vector2.Distance(transform.position, targetEnemy.transform.position);
                if (distance < ownProperties.attackRadius) {
                    if (curReloadingTime >= ownProperties.reloadingTime) {
                        curReloadingTime = 0;
                        // shoot
                        Vector2 direction = (targetEnemy.transform.position - source.transform.position).normalized;
                        Bullet bullet = BulletManager.GetObjectPooled();
                        bullet.transform.position = source.transform.position;
                        float lifeTime = 1.0f * ownProperties.attackRadius / ownProperties.bulletSpeed;
                        bullet.Shoot(new BulletProperties(Team, ownProperties.bulletSpeed, ownProperties.damage, direction), lifeTime);
                    }
                }
                else {
                    targetEnemy = null;
                }
            }
            curReloadingTime += Time.deltaTime;
        }
    }
    private List<Entity> GetEnemies() {
        List<Entity> list = new List<Entity>();
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, ownProperties.attackRadius);
        foreach (Collider2D collider in collider2Ds) {
            if (collider.TryGetComponent(out Entity entity) && entity.TryGetComponent(out IDamagable damagable)) {
                if (damagable.Team != Team) {
                    list.Add(entity);
                    break;
                }
            }
        }
        return list;
    }

    public void OnSelected() {
        selectorObj.SetActive(true);
        healthBar.Show();
        uiControl.Show();
    }
    public void OnDeselected() {
        selectorObj.SetActive(false);
        healthBar.Hide();
        uiControl.Hide();
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
        BuildingManager.current.LawnSprinklerBuildings.Remove(this);
    }

    public void OnGiveOrder(Vector2 position) {
        OnDeselected();
    }
    public void OnShowControlUI(bool isShow) {
        if (isShow) uiControl.Show();
        else uiControl.Hide();
    }

    public LawnSpinklerBuildingSaveData GetSaveObjectData() {
        return new LawnSpinklerBuildingSaveData() {
            building = new BuildingSaveData() {
                maxHealthPoint = properties.maxHealthPoint,
                curHealthPoint = properties.curHealthPoint,
                position = new SaveSystemExtension.Vector2(transform.position)
            }
        };
    }

    [System.Serializable]
    public class OwnProperties {
        public int attackRadius;
        public float reloadingTime;
        public int bulletSpeed;
        public int damage;
    }
}

[System.Serializable]
public struct LawnSpinklerBuildingSaveData {
    public BuildingSaveData building;
}