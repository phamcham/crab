using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public abstract class Unit : MonoBehaviour {
    [Header("Unit Settings")]
    [SerializeField] UnitType unitType;
    public BoundsInt area = new BoundsInt(Vector3Int.zero, Vector3Int.one);
    public UnitProperties Properties { get; set; }
    protected void Awake(){
        OnAwake();
    }
    protected abstract void OnAwake();
    protected void Start(){
        UnitManager.current.AddUnit(this);
        OnStart();
    }
    protected abstract void OnStart();
    public UnitType GetUnitType(){
        return unitType;
    }
    public abstract void OnSelected();
    public abstract void OnDeselected();
}

public class UnitProperties{
    public Team team;
    public int healthPoint;
    public int moveSpeed;
    public int damage;

    public UnitProperties(Team team, int healthPoint, int damage, int moveSpeed){
        this.team = team;
        this.healthPoint = healthPoint;
        this.damage = damage;
        this.moveSpeed = moveSpeed;
    }
}