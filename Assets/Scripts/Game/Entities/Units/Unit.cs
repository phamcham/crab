using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public abstract class Unit : Entity {
    public UnitProperties properties;
    [SerializeField] protected SpriteRenderer spriteRenderer;
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