using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public abstract class Unit : Entity {
    public UnitProperties properties;
    [SerializeField] SpriteRenderer spriteRenderer;
    protected void Awake(){
        properties.curHealthPoint = properties.maxHealthPoint;
        OnAwake();
    }
    protected abstract void OnAwake();
    protected void Start(){
        UnitManager.current.AddUnit(this);
        OnStart();
    }
    protected abstract void OnStart();
    public Sprite GetSprite() {
        return spriteRenderer.sprite;
    }
    protected void OnDestroy() {
        UnitManager.current.RemoveUnit(this);
        OnUnitDestroy();
    }
    protected abstract void OnUnitDestroy();
    //public abstract void ShowControlUI(bool active);
    //public abstract void OnSelected();
    //public abstract void OnDeselected();
}
[System.Serializable]
public class UnitProperties {
    public string unitName;
    public int maxHealthPoint;
    public int moveSpeed;
    public int damage;
    public BoundsInt area = new BoundsInt(Vector3Int.zero, Vector3Int.one);
    [HideInInspector]
    public int curHealthPoint;
}