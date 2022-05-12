using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {
    public static BulletManager instance { get; private set; }
    [SerializeField] private Bullet bulletPrefab;
    //private Transform holder;
    private Stack<Bullet> pooling = new Stack<Bullet>();
    private void Awake() {
        instance = this;
    }

    public static Bullet GetBulletPooled() {
        Bullet bullet = instance.pooling.Count > 0 ? instance.pooling.Pop() :
                Instantiate(instance.bulletPrefab.gameObject, instance.transform).GetComponent<Bullet>();
        bullet.gameObject.SetActive(true);
        return bullet;
    }
    public static void ReturnBulletPooled(Bullet bullet) {
        bullet.gameObject.SetActive(false);
        instance.pooling.Push(bullet);
    }
}
