using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridResourcesSystem : MonoBehaviour
{
    public static GridResourcesSystem current { get; private set; }
    [SerializeField] Tilemap resourceTilemap;
    Dictionary<ResourceType, TileBase> tilebaseByType = new Dictionary<ResourceType, TileBase>();
    Dictionary<TileBase, ResourceType> typesByTilebase = new Dictionary<TileBase, ResourceType>();
    Dictionary<ResourceType, List<Vector2Int>> resourcePositions = new Dictionary<ResourceType, List<Vector2Int>>();
    private void Awake() {
        current = this;
    }

    private void Start() {
        const string resourcePath = @"Resources Type/";
        tilebaseByType.Add(ResourceType.None, null);
        tilebaseByType.Add(ResourceType.Coconut, Resources.Load<TileBase>(resourcePath + ResourceType.Coconut.ToString().ToLower()));
        tilebaseByType.Add(ResourceType.Conch, Resources.Load<TileBase>(resourcePath + ResourceType.Conch.ToString().ToLower()));
        tilebaseByType.Add(ResourceType.Grass, Resources.Load<TileBase>(resourcePath + ResourceType.Grass.ToString().ToLower()));
        tilebaseByType.Add(ResourceType.Starfish, Resources.Load<TileBase>(resourcePath + ResourceType.Starfish.ToString().ToLower()));

        foreach (KeyValuePair<ResourceType, TileBase> res in tilebaseByType){
            if (res.Value != null){
                typesByTilebase.Add(res.Value, res.Key);
            }
        }

        CheckResourcesAvailableOnTilemap();
        foreach (ResourceType type in GetCurrentResourcesAvailable()){
            Tile tile = tilebaseByType[type] as Tile;
            Debug.Assert(UIResourcePrioritiesManager.current != null);
            UIResourcePrioritiesManager.current.AddSetter(type, tile.sprite, type.ToString(), 0);
        }
    }

    private void CheckResourcesAvailableOnTilemap(){
        BoundsInt miningArea = resourceTilemap.cellBounds;
        foreach (Vector2Int pos in miningArea.allPositionsWithin){
            TileBase tile = resourceTilemap.GetTile((Vector3Int)pos);
            if (tile != null){
                ResourceType type = typesByTilebase[tile];
                if (type != ResourceType.None){
                    if (!resourcePositions.ContainsKey(type)){
                        resourcePositions.Add(type, new List<Vector2Int>());
                    }
                    resourcePositions[type].Add(pos);
                }
            }
        }
    }

    public List<Vector2Int> GetResourcePositions(ResourceType type){
        if (resourcePositions.TryGetValue(type, out List<Vector2Int> res)){
            return res;
        }
        return null;
    }
    public List<ResourceType> GetCurrentResourcesAvailable(){
        if (resourcePositions.Count == 0){
            return null;
        }
        return new List<ResourceType>(resourcePositions.Keys);
    }
}

public enum ResourceType{
    None,
    Coconut,
    Conch,
    Grass,
    Starfish
}
