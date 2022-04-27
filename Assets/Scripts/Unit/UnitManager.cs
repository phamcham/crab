using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    public static UnitManager current { get; private set; }
    [SerializeField] List<Unit> unitPrefabs;
    Dictionary<UnitType, Unit> dictUnitPrefabs = new Dictionary<UnitType, Unit>();
    private void Awake() {
        current = this;
        foreach (Unit unit in unitPrefabs){
            dictUnitPrefabs.Add(unit.GetUnitType(), unit);
        }
    }
    public Unit Create(UnitType type) {
        if (type == UnitType.None) return null;
        return Instantiate(dictUnitPrefabs[type]);
    }
}

public enum UnitType {
    None,
    Crab
}