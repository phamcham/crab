using System;
using System.Collections.Generic;

public class MiningPlanManager {
    private static MiningPlanManager _current;
    public static MiningPlanManager current{
        get{
            if (_current == null) _current = new MiningPlanManager();
            return _current;
        }
    }
    private Dictionary<ResourceType, int> priorities = new Dictionary<ResourceType, int>();

    public int GetPriority(ResourceType type){
        if (priorities.TryGetValue(type, out int p)){
            return p;
        }
        return 0;
    }
    public void SetPriority(ResourceType type, int p){
        if (!priorities.ContainsKey(type)) priorities.Add(type, p);
        else priorities[type] = p;
    }
}