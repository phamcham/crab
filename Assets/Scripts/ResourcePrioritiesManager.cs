using System;
using System.Collections.Generic;
using System.Linq;
using PhamCham.Extension;
using UnityEngine;

public class ResourcePrioritiesManager {
    private static ResourcePrioritiesManager _current;
    public static ResourcePrioritiesManager current{
        get{
            if (_current == null) _current = new ResourcePrioritiesManager();
            return _current;
        }
    }
    private List<Gatherer> subscribedUnits = new List<Gatherer>();

    public void UpdateResourcePriorities(){
        List<UIE_ResourcePrioritySetter> setters = UI_ResourcesManager.current.GetAllSetters();
        // ra lenh cho cac unit o day
        int unitCount = subscribedUnits.Count;

        List<Gatherer> units = new List<Gatherer>(subscribedUnits);
        units.PCShuffer();
        Stack<Gatherer> queue = new Stack<Gatherer>(units);
        string s = queue.Count + " : ";

        int remain = units.Count;
        foreach (UIE_ResourcePrioritySetter setter in setters){
            int priority = setter.Priority;
            priority = Mathf.Clamp(priority, 0, remain);

            for (int i = 0; i < priority; i++){
                Gatherer gatherer = queue.Pop();
                gatherer.OnChangeTargetResourceType(setter.ResourceType);
            }
            remain -= priority;
        }
        while (queue.Count > 0){
            Gatherer gatherer = queue.Pop();
            gatherer.OnChangeTargetResourceType(ResourceType.None);
        }

        foreach (UIE_ResourcePrioritySetter setter in setters) {
            setter.numberSpinner.MaxValue = setter.numberSpinner.Value + remain;
        }

        UI_ResourcesManager.current.UpdateText($"{remain}/{unitCount}");
        
    }
    public void SubscribeGathererUnit(Gatherer unit){
        subscribedUnits.Add(unit);

        UpdateResourcePriorities();
    }
    public void UnsubcribeGatherUnit(Gatherer unit){
        if (Application.isPlaying){
            subscribedUnits.Remove(unit);

            UpdateResourcePriorities();
        }
    }
}