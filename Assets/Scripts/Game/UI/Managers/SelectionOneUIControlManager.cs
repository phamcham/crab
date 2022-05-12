using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionOneUIControlManager : MonoBehaviour {
    public static SelectionOneUIControlManager current { get; private set; }
    [SerializeField] Transform controlUIHolder;
    [Header("Unit UI Prefabs")]
    [SerializeField] UIE_GatheringCrabUnitControl crabUnitControlUIPrefab;
    [SerializeField] UIE_BubbleCrabUnitControl bubbleCrabUnitControlUIPrefab;
    [SerializeField] UIE_HermitCrabUnitControl hermitCrabUnitControlUIPrefab;
    [Header("Building UI Prefabs")]
    [SerializeField] UIE_HouseBuildingControl houseControlUIPrefab;
    [SerializeField] UIE_HeadquarterBuildingControl headquarterControlUIPrefab;
    [SerializeField] UIE_LawnSprinklerBuildingControl lawnSprinklerControlUIPrefab;
    [SerializeField] UIE_SandWallBuildingControl sandWallControlUIPrefab;
    [Header("Resource UI Prefabs")]
    [SerializeField] UIE_ResourceControl resourceControlUIPrefabs;
    //
    Dictionary<Type, UIE_UIControl> dictPrefabs = new Dictionary<Type, UIE_UIControl>();
    Dictionary<Entity, UIE_UIControl> controls = new Dictionary<Entity, UIE_UIControl>();
    HashSet<UIE_UIControl> set = new HashSet<UIE_UIControl>();
    private void Awake() {
        current = this;
        
        dictPrefabs = new Dictionary<Type, UIE_UIControl>() {
            // crabs
            { typeof(UIE_GatheringCrabUnitControl),  crabUnitControlUIPrefab },
            { typeof(UIE_BubbleCrabUnitControl), bubbleCrabUnitControlUIPrefab },
            { typeof(UIE_HermitCrabUnitControl), hermitCrabUnitControlUIPrefab },
            // buildings
            { typeof(UIE_HouseBuildingControl),  houseControlUIPrefab },
            { typeof(UIE_HeadquarterBuildingControl),  headquarterControlUIPrefab },
            { typeof(UIE_LawnSprinklerBuildingControl), lawnSprinklerControlUIPrefab },
            { typeof(UIE_SandWallBuildingControl), sandWallControlUIPrefab },
            // resources
            { typeof(UIE_ResourceControl), resourceControlUIPrefabs }
        };
    }
    private void Start() {
    }
    public Transform GetHolder(){
        return controlUIHolder;
    }
    public T GetUIControl<T>(Entity owner) where T : UIE_UIControl {
        if (!dictPrefabs.TryGetValue(typeof(T), out UIE_UIControl prefab)) {
            Debug.LogError("Chua them " + nameof(T));
            return null;
        }
        //UIE_UIControl prefab = dictPrefabs[typeof(T)];
        if (!controls.TryGetValue(owner, out UIE_UIControl control)) {
            control = Instantiate(prefab.gameObject, controlUIHolder).GetComponent<T>();
            control.name = $"{typeof(T).Name} ({owner.GetInstanceID()})";
            controls.Add(owner, control);
        }
        return control.GetComponent<T>();
    }
    public void RemoveUIControl(Entity owner) {
        //print("remove " + owner.GetInstanceID());
        if (controls.TryGetValue(owner, out UIE_UIControl control)) {
            if (control && control.gameObject){
                Destroy(control.gameObject);
            }
        }
        controls.Remove(owner);
    }
}
