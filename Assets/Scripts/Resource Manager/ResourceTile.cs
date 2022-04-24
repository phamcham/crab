using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceTile {
    public ResourceType type;
    public Tile tile;
    public Vector2Int position;
    public int amount;

    public ResourceTile(ResourceType type, Tile tile, Vector2Int position, int amount){
        this.type = type;
        this.tile = tile;
        this.position = position;
        this.amount = amount;
    }
}