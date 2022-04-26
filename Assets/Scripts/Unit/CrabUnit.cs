using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CrabUnit : Unit  {
    
    [Header("Crab settings")]
    [SerializeField] Transform spriteTrans;
    //UnitTask[] unitTasks;

    UnitMovement movement;

    protected override void OnAwake() {
        Properties = new UnitProperties(Team.DefaultPlayer, 100, 10, 6);
        //unitTasks = GetComponents<UnitTask>();
        movement = GetComponent<UnitMovement>();
    }
    protected override void OnStart() {
        movement.MoveToPosition(transform.position);
    }
    /*public override void OnMove(Vector2 direction){
        if (direction != Vector2.zero){
            float angle = Vector2.SignedAngle(Vector2.up, direction);
            spriteTrans.DOKill();
            spriteTrans.DOLocalRotate(new Vector3(0, 0, angle), 0.3f).Play();
        }
    }*/
}