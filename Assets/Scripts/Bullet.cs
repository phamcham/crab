using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] Rigidbody2D rigid;
    public int speed;
    private int damage;
    Vector2 direction;
    public void Shoot(int damage, Vector2 direction, int speed) {
        this.damage = damage;
        this.direction = direction;
        this.speed = speed;

        rigid.velocity = direction.normalized * speed;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == gameObject) return;
        if (other.TryGetComponent(out UnitDamagable damagable)) {
            damagable.TakeDamage(damage);
            BulletManager.ReturnBulletPooled(this);
        }
    }
}