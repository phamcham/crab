[System.Serializable]
public class ResourceRequirement {
    //public ResourceData data;
    public ResourceType type;
    public int amount;
    public ResourceRequirement(ResourceType type, int amount){
        //this.data = data;
        this.type = type;
        this.amount = amount;
    }
}