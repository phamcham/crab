using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling<ObjectType> : MonoBehaviour where ObjectType : MonoBehaviour{
    public static ObjectPooling<ObjectType> instance { get; private set; }
    [SerializeField] private ObjectType prefab;
    private void Awake() {
        instance = this;
    }
    protected Stack<ObjectType> pooling = new Stack<ObjectType>();
    public static ObjectType GetObjectPooled() {
        print("GetObjectPooled: " + instance.pooling.Count);
        ObjectType obj = instance.pooling.Count > 0 ? instance.pooling.Pop() :
                Instantiate(instance.prefab.gameObject, instance.transform).GetComponent<ObjectType>();
        obj.gameObject.SetActive(true);
        return obj;
    }
    public static void ReturnBulletPooled(ObjectType obj) {
        print("ReturnBulletPooled: " + instance.pooling.Count);
        obj.gameObject.SetActive(false);
        instance.pooling.Push(obj);
    }
}
