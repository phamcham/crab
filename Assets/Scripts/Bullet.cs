using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] Rigidbody2D rigid;
    BulletProperties properties;
    float lifeTime;
    bool isLiving = false;
    public void Shoot(BulletProperties bulletProperties, float lifeTime) {
        this.properties.owner = bulletProperties.owner;
        this.properties.damage = bulletProperties.damage;
        this.properties.direction = bulletProperties.direction;
        this.properties.speed = bulletProperties.speed;
        this.lifeTime = lifeTime;
        isLiving = true;

        rigid.velocity = properties.direction.normalized * properties.speed;
    }
    private void Update() {
        if (isLiving) {
            if (lifeTime > 0) {
                lifeTime -= Time.deltaTime;
            }
            else {
                isLiving = false;
                BulletManager.ReturnBulletPooled(this);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == gameObject) return;
        if (other.TryGetComponent(out IDamagable unit)) {
            if (unit.Team != properties.owner) {
                unit.TakeDamage(properties.damage);
                BulletManager.ReturnBulletPooled(this);
            }
        }
    }
}

public struct BulletProperties {
    public Team owner;
    public int speed;
    public int damage;
    public Vector2 direction;

    public BulletProperties(Team owner, int speed, int damage, Vector2 direction) {
        this.owner = owner;
        this.speed = speed;
        this.damage = damage;
        this.direction = direction;
    }
}