using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public abstract class Unit : Entity {
    public UnitProperties properties;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public abstract UnitType type { get; }
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

[System.Serializable]
public struct UnitSaveData {
    public int maxHealthPoint;
    public int curHealthPoint;
    public int moveSpeed;
    public int damage;
    public SaveSystemExtension.Vector2 position;
}