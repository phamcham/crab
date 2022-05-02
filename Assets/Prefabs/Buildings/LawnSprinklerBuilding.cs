using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LawnSprinklerBuilding : Building, IDamagable {
    public override Team Team => Team.DefaultPlayer;
    public OwnProperties ownProperties;
    private BuildingSelectable selectable;
    bool isStartShooting = false;
    const float checkInterval = 1f;
    float checkTime = 0;
    float curReloadingTime = 0;
    EnemyUnit targetEnemy;
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject selectorObj;
    [SerializeField] Transform source; // nong sung
    public override void OnBuildingPlaced() {
        isStartShooting = true;
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
                    List<EnemyUnit> enemies = GetEnemies();
                    if (enemies.Count > 0) {
                        targetEnemy = enemies[0];
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
                        Bullet bullet = BulletManager.GetBulletPooled();
                        bullet.transform.position = source.transform.position;
                        float lifeTime = 1.0f * ownProperties.attackRadius / ownProperties.bulletSpeed;
                        bullet.Shoot(new BulletProperties(Team, ownProperties.bulletSpeed, ownProperties.damage, direction), lifeTime);
                    }
                    else {
                        curReloadingTime += Time.deltaTime;
                    }
                }
                else {
                    targetEnemy = null;
                }
            }
        }
    }
    private List<EnemyUnit> GetEnemies() {
        List<EnemyUnit> list = new List<EnemyUnit>();
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, ownProperties.attackRadius);
        foreach (Collider2D collider in collider2Ds) {
            if (collider.TryGetComponent(out EnemyUnit damagable)) {
                if (damagable.Team != Team) {
                    list.Add(damagable);
                    break;
                }
            }
        }
        return list;
    }
    
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
        print("???");
        selectorObj.SetActive(false);
    }
    private void OnShowControlUIHandle(bool active){
        UIE_LawnSprinklerBuildingControl uie = SelectionOneUIControlManager.current.GetUIControl<UIE_LawnSprinklerBuildingControl>(this);
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
    public class OwnProperties {
        public int attackRadius;
        public float reloadingTime;
        public int bulletSpeed;
        public int damage;
    }
}
