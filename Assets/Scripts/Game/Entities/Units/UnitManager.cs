using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    public static UnitManager current { get; private set; }
    [SerializeField] GatheringCrabUnit gatheringCrabUnitPrefab;
    [SerializeField] BubbleCrabUnit bubbleCrabUnitPrefab;
    [SerializeField] HermitCrabUnit hermitCrabUnitPrefab;
    // cua storing nhung dung chung de hien thi unit
    [SerializeField] Transform storingHolder;
    [SerializeField] UIE_NumberOfThings uie_numberOfThingsPrefab;
    [SerializeField] Sprite crabIcon;
    [SerializeField] Sprite houseIcon;
    UIE_NumberOfThings uie_numberOfCrabs, uie_houseCapacity;
    Dictionary<UnitType, Unit> dictUnitPrefabs = new Dictionary<UnitType, Unit>();
    //List<Unit> units = new List<Unit>();
    [HideInInspector] public int houseCapacity;
    public int UnitCount {
        get {
            return GatheringCrabUnits.Count + BubbleCrabUnits.Count + HermitCrabUnits.Count;
        }
    }

    public List<GatheringCrabUnit> GatheringCrabUnits { get; set; } = new List<GatheringCrabUnit>();
    public List<BubbleCrabUnit> BubbleCrabUnits { get; set; } = new List<BubbleCrabUnit>();
    public List<HermitCrabUnit> HermitCrabUnits { get; set; } = new List<HermitCrabUnit>();
    private void Awake() {
        current = this;
    }
    private void Start() {
        dictUnitPrefabs = new Dictionary<UnitType, Unit>() {
            { UnitType.Gathering, gatheringCrabUnitPrefab },
            { UnitType.Bubble, bubbleCrabUnitPrefab },
            { UnitType.Hermit, hermitCrabUnitPrefab }
        };
        uie_numberOfCrabs = Instantiate(uie_numberOfThingsPrefab.gameObject, storingHolder).GetComponent<UIE_NumberOfThings>();
        uie_houseCapacity = Instantiate(uie_numberOfThingsPrefab.gameObject, storingHolder).GetComponent<UIE_NumberOfThings>();
        UpdateUnitAmountUI();
        UpdateHouseCapacityUI();
    }
    public Unit Create(UnitType type) {
        Unit obj = Instantiate(dictUnitPrefabs[type].gameObject, transform).GetComponent<Unit>();
        //UpdateUnitAmountUI();
        return obj;
    }
    // public void AddUnit(Unit unit) {
    //     if (!units.Contains(unit)) {
    //         units.Add(unit);
    //     }
    //     UpdateUnitAmountUI();
    // }
    public void ChangeHouseCapacity(int add) {
        houseCapacity += add;
        UpdateHouseCapacityUI();
    }
    public void UpdateUnitAmountUI() {
        if (uie_numberOfCrabs) {
            print(UnitCount);
            uie_numberOfCrabs.Setup(crabIcon, UnitCount);
        }
    }
    public void UpdateHouseCapacityUI() {
        if (uie_houseCapacity) {
            uie_houseCapacity.Setup(houseIcon, houseCapacity);
        }
    }
}