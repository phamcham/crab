using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionOneUIControlManager : MonoBehaviour {
    public static SelectionOneUIControlManager current { get; private set; }
    [SerializeField] Transform controlUIHolder;
    [Header("Prefabs")]
    [SerializeField] UIE_CrabUnitControl crabUnitControlUIPrefab;
    [SerializeField] UIE_BubbleCrabUnitControl bubbleCrabUnitControlUIPrefab;
    [SerializeField] UIE_HouseBuildingControl houseControlUIPrefab;
    [SerializeField] UIE_HeadquarterBuildingControl headquarterControlUIPrefab;
    [SerializeField] UIE_LawnSprinklerBuildingControl lawnSprinklerControlUIPrefabs;
    [SerializeField] UIE_ResourceControl resourceControlUIPrefabs;
    //
    Dictionary<Type, UIE_UIControl> dictPrefabs = new Dictionary<Type, UIE_UIControl>();
    Dictionary<Entity, UIE_UIControl> controls = new Dictionary<Entity, UIE_UIControl>();
    HashSet<UIE_UIControl> set = new HashSet<UIE_UIControl>();
    private void Awake() {
        current = this;
        
        dictPrefabs = new Dictionary<Type, UIE_UIControl>() {
            { typeof(UIE_CrabUnitControl),  crabUnitControlUIPrefab },
            { typeof(UIE_BubbleCrabUnitControl), bubbleCrabUnitControlUIPrefab },
            { typeof(UIE_HouseBuildingControl),  houseControlUIPrefab },
            { typeof(UIE_HeadquarterBuildingControl),  headquarterControlUIPrefab },
            { typeof(UIE_LawnSprinklerBuildingControl), lawnSprinklerControlUIPrefabs },
            { typeof(UIE_ResourceControl), resourceControlUIPrefabs }
        };
    }
    private void Start() {
    }
    public Transform GetHolder(){
        return controlUIHolder;
    }
    public T GetUIControl<T>(Entity owner) where T : UIE_UIControl{
        UIE_UIControl prefab = dictPrefabs[typeof(T)];
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
