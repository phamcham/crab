using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEnemyCrabUnit : EnemyUnit, IDamagable, ISaveObject<BubbleEnemyCrabUnitSaveData> {
    public override UnitType type => UnitType.Bubble;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] bool targetHeadquarter;
    private UnitNavMovement movement;
    private EnemyUnitShootable shootable;
    const float checkInterval = 2f;
    private float curCheck = 0;
    private Entity target;
    private float fixedOrbit;
    private bool stopToShoot;
    FlashOnImpactEffect impactEffect;


    protected override void OnAwake() {
        movement = GetComponent<UnitNavMovement>();
        shootable = GetComponent<EnemyUnitShootable>();

        if (!base.spriteRenderer.TryGetComponent(out impactEffect)) {
            impactEffect = base.spriteRenderer.gameObject.AddComponent<FlashOnImpactEffect>();
        }
    }

    protected override void OnStart() {
        fixedOrbit = Random.Range(0.5f, 0.9f) * shootable.attackRadius;

        healthBar.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        healthBar.Show();
    }

    private void Update() {
        if (target == null) {
            if (curCheck < checkInterval) {
                curCheck += Time.deltaTime;
            }
            else {
                curCheck = 0;
                if (targetHeadquarter) target = BuildingManager.current.GetHeadquarterBuilding();
                else target = GetNearestUnit(GetUnitDamagables(shootable.attackRadius * 1.5f));
            }
        }
        else {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance >= fixedOrbit) {
                stopToShoot = false;
                movement.MoveToPosition(target.transform.position);
            }
            else {
                if (!stopToShoot) {
                    stopToShoot = true;
                    movement.StopMovement();
                }
            }
        }
    }

    private List<Unit> GetUnitDamagables(float radius) {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        List<Unit> list = new List<Unit>();
        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out Unit unit)) {
                if (unit.TryGetComponent(out IDamagable damagable)) {
                    if (damagable.Team != Team){
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
            float distance = Vector2.Distance(transform.position, unit.transform.position);
            if (!res || distance < minDistance) {
                minDistance = distance;
                res = unit;
            }
        }
        return res;
    }

    protected override void OnUnitDestroy() {
        // TODO: hieu ung pha huy
    }

    public void TakeDamage(int damage) {
        int curHeath = properties.curHealthPoint;
        int maxHeath = properties.maxHealthPoint;
        //print("cuerh: " + curHeath + "/" + maxHeath);
        curHeath -= damage;
        if (curHeath < 0) curHeath = 0;
        properties.curHealthPoint = curHeath;

        healthBar.SetSize(1.0f * curHeath / maxHeath);
        healthBar.Show();

        impactEffect.Impact();
        if (curHeath == 0) {
            // die for you
            Destroy(gameObject);
        }
    }

    public void SetTarget(bool isHeadquarter) {
        targetHeadquarter = isHeadquarter;
    }

    public BubbleEnemyCrabUnitSaveData GetSaveObjectData() {
        return new BubbleEnemyCrabUnitSaveData() {
            unit = new UnitSaveData() {
                maxHealthPoint = properties.maxHealthPoint,
                curHealthPoint = properties.curHealthPoint,
                moveSpeed = properties.moveSpeed,
                damage = properties.damage,
                position = new SaveSystemExtension.Vector2(transform.position)
            }
        };
    }
}

[System.Serializable]
public struct BubbleEnemyCrabUnitSaveData {
    public UnitSaveData unit;
}