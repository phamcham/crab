using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEnemyCrabUnit : EnemyUnit, IDamagable {
    [SerializeField] private HealthBar healthBar;
    private UnitMovement movement;
    private EnemyUnitShootable shootable;
    const float checkInterval = 2f;
    private float curCheck = 0;
    private HeadquarterBuilding headquarter;
    private float fixedOrbit;
    private bool stopToShoot;
    FlashOnImpactEffect impactEffect;
    protected override void OnAwake() {
        movement = GetComponent<UnitMovement>();
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
        if (!headquarter) {
            if (curCheck < checkInterval) {
                curCheck += Time.deltaTime;
            }
            else {
                curCheck = 0;
                headquarter = GameController.current.GetHeadquarterBuilding();
            }
        }
        else {
            float distance = Vector2.Distance(transform.position, headquarter.transform.position);
            if (distance >= fixedOrbit) {
                stopToShoot = false;
                movement.MoveToPosition(headquarter.transform.position);
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
}
