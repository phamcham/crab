using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    public static UnitManager current { get; private set; }
    [SerializeField] CrabUnit crabUnitPrefab;
    // cua storing nhung dung chung de hien thi unit
    [SerializeField] Transform storingHolder;
    [SerializeField] UIE_NumberOfThings uie_numberOfThingsPrefab;
    UIE_NumberOfThings uie_numberOfCrabs;
    Dictionary<Type, Unit> dictUnitPrefabs = new Dictionary<Type, Unit>();
    List<Unit> units = new List<Unit>();
    private void Awake() {
        current = this;
    }
    private void Start() {
        dictUnitPrefabs = new Dictionary<Type, Unit>() {
            { typeof(CrabUnit), crabUnitPrefab }
        };
        uie_numberOfCrabs = Instantiate(uie_numberOfThingsPrefab.gameObject, storingHolder)
                                .GetComponent<UIE_NumberOfThings>();
        UpdateUnitAmountUI();
    }
    public T Create<T>() where T : Unit {
        T obj = Instantiate(dictUnitPrefabs[typeof(T)].gameObject).GetComponent<T>();
        
        return obj;
    }
    public void AddUnit(Unit unit) {
        if (!units.Contains(unit)) {
            units.Add(unit);
        }
        UpdateUnitAmountUI();
    }
    public void RemoveUnit(Unit unit) {
        if (unit != null) {
            units.Remove(unit);
            UpdateUnitAmountUI();
            
        }
    }
    private void UpdateUnitAmountUI() {
        if (crabUnitPrefab && uie_numberOfCrabs) {
            uie_numberOfCrabs.Setup(crabUnitPrefab.GetSprite(), units.Count);
        }
    }
}