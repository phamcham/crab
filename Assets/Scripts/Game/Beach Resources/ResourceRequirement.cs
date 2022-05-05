using UnityEngine;

[System.Serializable]
public class ResourceRequirement {
    //public ResourceData data;
    public ResourceType type;
    public int amount;
    [HideInInspector] public bool enough;
    public ResourceRequirement(ResourceType type, int amount, bool enough){
        //this.data = data;
        this.type = type;
        this.amount = amount;
        this.enough = enough;
    }
}