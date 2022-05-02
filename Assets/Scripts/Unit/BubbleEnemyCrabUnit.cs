using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEnemyCrabUnit : EnemyUnit, IDamagable {
    [SerializeField] HealthBar healthBar;
    UnitMovement movement;
    EnemyUnitShootable shootable;
    const float checkInterval = 2f;
    float curCheck = 0;
    HeadquarterBuilding headquarter;
    float fixedOrbit;
    bool stopToShoot;
    protected override void OnAwake() {
        movement = GetComponent<UnitMovement>();
        shootable = GetComponent<EnemyUnitShootable>();
    }

    protected override void OnStart() {
        fixedOrbit = Random.Range(0.5f, 0.9f) * shootable.attackRadius;
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

    protected override void OnUnitDestroy()
    {
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
}
