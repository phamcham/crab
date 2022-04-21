using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResourcePrioritiesManager : MonoBehaviour {
    public static UIResourcePrioritiesManager current { get; private set; }

    [SerializeField] ResourcePrioritySetter setterPrefab;
    [SerializeField] Transform content;
    Dictionary<ResourceType, ResourcePrioritySetter> setters = new Dictionary<ResourceType, ResourcePrioritySetter>();
    private List<Unit> subscribedUnits = new List<Unit>();
    private void Awake() {
        current = this;
    }
    public void AddSetter(ResourceType resourceType, Sprite sprite, string name, int priority){
        if (setters.ContainsKey(resourceType)) {
            Debug.LogWarning(resourceType + " already added!");
            return;
        }
        ResourcePrioritySetter setter = Instantiate(setterPrefab, content).GetComponent<ResourcePrioritySetter>();
        setter.Setup(resourceType, sprite, name, priority);
        setters.Add(resourceType, setter);
    }

    public List<ResourcePrioritySetter> GetAllSetters(){
        return new List<ResourcePrioritySetter>(setters.Values);
    }

    public void UpdateResourcePriorities(){
        List<ResourcePrioritySetter> setters = GetAllSetters();
        // ra lenh cho cac unit o day
    }

    //TODO: change unit to something like GathererUnit (ref: WarriorUnit)
    public void SubscribeGathererUnit(Unit unit){
        subscribedUnits.Add(unit);
    }
    public void UnsubcribeGatherUnit(Unit unit){
        subscribedUnits.Remove(unit);
    }
}
