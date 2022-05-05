using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    public static UnitManager current { get; private set; }
    [SerializeField] GatheringCrabUnit crabUnitPrefab;
    // cua storing nhung dung chung de hien thi unit
    [SerializeField] Transform storingHolder;
    [SerializeField] UIE_NumberOfThings uie_numberOfThingsPrefab;
    [SerializeField] Sprite crabIcon;
    [SerializeField] Sprite houseIcon;
    UIE_NumberOfThings uie_numberOfCrabs, uie_houseCapacity;
    Dictionary<Type, Unit> dictUnitPrefabs = new Dictionary<Type, Unit>();
    List<Unit> units = new List<Unit>();
    [HideInInspector] public int houseCapacity;
    [HideInInspector] public int unitCount => units.Count;
    Transform unitHolder;
    private void Awake() {
        current = this;

        unitHolder = new GameObject("Unit Holder").transform;
        unitHolder.transform.position = Vector3.zero;
    }
    private void Start() {
        dictUnitPrefabs = new Dictionary<Type, Unit>() {
            { typeof(GatheringCrabUnit), crabUnitPrefab }
        };
        uie_numberOfCrabs = Instantiate(uie_numberOfThingsPrefab.gameObject, storingHolder).GetComponent<UIE_NumberOfThings>();
        uie_houseCapacity = Instantiate(uie_numberOfThingsPrefab.gameObject, storingHolder).GetComponent<UIE_NumberOfThings>();
        UpdateUnitAmountUI();
        UpdateHouseCapacityUI();
    }
    public T Create<T>() where T : Unit {
        T obj = Instantiate(dictUnitPrefabs[typeof(T)].gameObject, unitHolder).GetComponent<T>();
        
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
    public void ChangeHouseCapacity(int add) {
        houseCapacity += add;
        UpdateHouseCapacityUI();
    }
    private void UpdateUnitAmountUI() {
        if (crabUnitPrefab && uie_numberOfCrabs) {
            uie_numberOfCrabs.Setup(crabIcon, units.Count);
        }
    }
    private void UpdateHouseCapacityUI() {
        if (uie_houseCapacity) {
            uie_houseCapacity.Setup(houseIcon, houseCapacity);
        }
    }
}