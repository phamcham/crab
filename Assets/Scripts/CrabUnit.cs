using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CrabUnit : Unit {
    protected override void Awake() {
        base.Awake();
        Properties = new UnitProperties(Team.DefaultPlayer, 100, 10, 2);
    }
    private void Start() {
        TargetPosition = transform.position;
    }
    public override void OnMove(Vector2 direction){
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        transform.DOKill();
        transform.DOLocalRotate(new Vector3(0, 0, angle), 0.3f).Play();
    }
}
