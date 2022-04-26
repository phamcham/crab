using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitTask : MonoBehaviour {
    public const int DoNothing = 1;
    protected Unit unit;
    protected int currentStep = -1;
    protected Dictionary<int, Action> steps = new Dictionary<int, Action>();
    protected void AddStep(int id, Action action){
        steps.Add(id, action);
    }
    protected void Awake() {
        unit = GetComponent<Unit>();
        OnAwake();
    }
    protected abstract void OnAwake();
    protected void Update(){
        if (currentStep >= 1){
            steps[currentStep]?.Invoke();
        }
        OnUpdate();
    }
    protected abstract void OnUpdate();

    public abstract void StartDoTask();
    public abstract void EndDoTask();
}
