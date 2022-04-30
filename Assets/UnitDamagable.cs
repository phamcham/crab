using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// unit can take damage and hurtful
public class UnitDamagable : MonoBehaviour {
    [SerializeField] HealthBar healthBar;
    public Unit BaseUnit { get; private set; }
    private void Awake() {
        BaseUnit = GetComponent<Unit>();
    }
    public void TakeDamage(int damage) {
        int curHeath = BaseUnit.properties.curHealthPoint;
        int maxHeath = BaseUnit.properties.maxHealthPoint;
        curHeath -= damage;
        if (curHeath < 0) curHeath = 0;
        BaseUnit.properties.curHealthPoint = curHeath;

        healthBar.SetSize(curHeath / maxHeath);

        if (curHeath == 0) {
            // TODO: die for you
        }
    }
}
