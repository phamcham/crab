using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// unit can take damage and hurtful
public class UnitDamagable : MonoBehaviour {
    [SerializeField] HealthBar healthBar;
    public Unit BaseUnit { get; private set; }
    private void Awake() {
        BaseUnit = GetComponent<Unit>();
    }
    private void Start() {
        healthBar.SetSize(1);
        SetDisplayHealthbar(false);
    }
    public void TakeDamage(int damage) {
        SetDisplayHealthbar(true);
        StopCoroutine(nameof(DoHideHealthbar));

        int curHeath = BaseUnit.properties.curHealthPoint;
        int maxHeath = BaseUnit.properties.maxHealthPoint;
        //print("cuerh: " + curHeath + "/" + maxHeath);
        curHeath -= damage;
        if (curHeath < 0) curHeath = 0;
        BaseUnit.properties.curHealthPoint = curHeath;

        healthBar.SetSize(1.0f * curHeath / maxHeath);

        if (curHeath == 0) {
            // TODO: die for you
        }
        else {
            StartCoroutine(nameof(DoHideHealthbar));
        }
    }
    IEnumerator DoHideHealthbar() {
        //Debug.Log("aasdasd");
        yield return new WaitForSeconds(4f);
        healthBar.gameObject.SetActive(false);
    }
    public void SetDisplayHealthbar(bool active) {
        if (active) {
            healthBar.gameObject.SetActive(true);
        }
        else {
            StopCoroutine(nameof(DoHideHealthbar));
            StartCoroutine(nameof(DoHideHealthbar));
        }
    }
}
