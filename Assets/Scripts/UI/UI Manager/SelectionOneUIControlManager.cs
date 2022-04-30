using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionOneUIControlManager : MonoBehaviour {
    public static SelectionOneUIControlManager current { get; private set; }
    [SerializeField] Transform controlUIHolder;
    [SerializeField] UIE_CrabUnitControl uieControlUIPrefab;
    [SerializeField] UIE_SpawnerBuildingControl spawnerControlUIPrefab;
    [SerializeField] UIE_HeadquarterBuildingControl headquarterControlUIPrefab;
    Dictionary<Type, UIE_UIControl> dictPrefabs = new Dictionary<Type, UIE_UIControl>();
    Dictionary<Entity, UIE_UIControl> controls = new Dictionary<Entity, UIE_UIControl>();
    HashSet<UIE_UIControl> set = new HashSet<UIE_UIControl>();
    private void Awake() {
        current = this;
    }
    private void Start() {
        dictPrefabs = new Dictionary<Type, UIE_UIControl>() {
            { typeof(UIE_CrabUnitControl),  uieControlUIPrefab },
            { typeof(UIE_SpawnerBuildingControl),  spawnerControlUIPrefab },
            { typeof(UIE_HeadquarterBuildingControl),  headquarterControlUIPrefab },
        };
    }
    public Transform GetHolder(){
        return controlUIHolder;
    }
    public T GetUIControl<T>(Entity owner) where T : UIE_UIControl{
        UIE_UIControl prefab = dictPrefabs[typeof(T)];
        if (!controls.TryGetValue(owner, out UIE_UIControl control)) {
            control = Instantiate(prefab.gameObject, controlUIHolder).GetComponent<T>();
            controls.Add(owner, control);
        }
        return control.GetComponent<T>();
    }
    public void RemoveUIControl(Entity owner) {
        if (!controls.TryGetValue(owner, out UIE_UIControl control)) {
            if (control && control.gameObject){
                Destroy(control.gameObject);
            }
        }
        controls.Remove(owner);
    }
}
