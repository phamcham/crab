using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] Rigidbody2D rigid;
    private Unit owner;
    public int speed;
    private int damage;
    private Vector2 direction;
    public void Shoot(Unit owner, int damage, Vector2 direction, int speed) {
        this.owner = owner;
        this.damage = damage;
        this.direction = direction;
        this.speed = speed;

        rigid.velocity = direction.normalized * speed;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == gameObject) return;
        if (other.TryGetComponent(out UnitDamagable damagable)) {
            if (damagable.BaseUnit.Team != owner.Team) {
                damagable.TakeDamage(damage);
                BulletManager.ReturnBulletPooled(this);
            }
        }
    }
}