using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    public static UnitManager current { get; private set; }
    [SerializeField] CrabUnit crabUnitPrefab;
    Dictionary<Type, Unit> dictUnitPrefabs = new Dictionary<Type, Unit>();
    List<Unit> units = new List<Unit>();
    private void Awake() {
        current = this;
    }
    private void Start() {
        dictUnitPrefabs = new Dictionary<Type, Unit>() {
            { typeof(CrabUnit), crabUnitPrefab }
        };
    }
    public T Create<T>() where T : Unit {
        return Instantiate(dictUnitPrefabs[typeof(T)].gameObject).GetComponent<T>();
    }
    // public void AddUnit(Unit unit) {
    //     if (units.Contains(unit)) return;
    //     units.Add(unit);
    // }
}