using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Resource Data", menuName = "ScriptableObjects/ResourceData", order = 1)]
public class ResourceData : ScriptableObject{
    public ResourceType type;
    public Tile tile;

    public ResourceData(ResourceType type, Tile tile)
    {
        this.type = type;
        this.tile = tile;
    }

    public ResourceData(ResourceData copy){
        this.type = copy.type;
        this.tile = copy.tile;
    }
}