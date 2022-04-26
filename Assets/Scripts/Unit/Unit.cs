using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public abstract class Unit : MonoBehaviour {
    [Header("Unit Settings")]
    public BoundsInt area = new BoundsInt(Vector3Int.zero, Vector3Int.one);
    public UnitProperties Properties { get; set; }
    protected void Awake(){
        OnAwake();
    }
    protected abstract void OnAwake();
    protected void Start(){
        OnStart();
    }
    protected abstract void OnStart();
}

public class UnitProperties{
    public Team team;
    public int healthPoint;
    public float moveSpeed;
    public int damage;

    public UnitProperties(Team team, int healthPoint, int damage, float moveSpeed){
        this.team = team;
        this.healthPoint = healthPoint;
        this.damage = damage;
        this.moveSpeed = moveSpeed;
    }
}