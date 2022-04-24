using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager current { get; private set; }
    //[SerializeField] UI_ResourcesManager ui;
    [SerializeField] Tilemap tilemapResource;
    [SerializeField] Tilemap tilemapGround;
    [SerializeField] List<ResourceData> resourceDatas;
    Dictionary<ResourceType, ResourceData> dictResourceData = new Dictionary<ResourceType, ResourceData>();
    Dictionary<ResourceType, List<ResourceTile>> dictResourceTiles = new Dictionary<ResourceType, List<ResourceTile>>();
    private void Awake() {
        current = this;
    }

    private void Start() {
        InitializeResourceDataDict();
        FillResourcesToTilemap();
        UpdateUI();
    }

    void InitializeResourceDataDict(){
        foreach (ResourceData resource in resourceDatas){
            dictResourceData.Add(resource.type, resource);
        }
    }

    void FillResourcesToTilemap() {
        // tmp
        List<Vector2Int> starfish_positions = new List<Vector2Int>(){
            new Vector2Int(-26, -13), new Vector2Int(-20, -13), new Vector2Int(-25, -12), new Vector2Int(-23, -12), 
            new Vector2Int(-22, -12), new Vector2Int(-26, -11), new Vector2Int(-25, -11), new Vector2Int(-24, -11), 
            new Vector2Int(-25, -10), new Vector2Int(-27, -9)
        };
        List<Vector2Int> grass_positions = new List<Vector2Int>(){
            new Vector2Int(15, -13), new Vector2Int(16, -13), new Vector2Int(21, -13), new Vector2Int(22, -13), 
            new Vector2Int(23, -13), new Vector2Int(17, -12), new Vector2Int(19, -12), new Vector2Int(22, -12), 
            new Vector2Int(23, -12), new Vector2Int(19, -11), new Vector2Int(20, -11), new Vector2Int(21, -11), 
            new Vector2Int(22, -11), new Vector2Int(16, -10), new Vector2Int(19, -10), new Vector2Int(23, -10), 
            new Vector2Int(19, -9), new Vector2Int(20, -9), new Vector2Int(23, -8), new Vector2Int(24, -7)
        };
        List<Vector2Int> conch_positions = new List<Vector2Int>(){
            new Vector2Int(21, 4), new Vector2Int(15, 6), new Vector2Int(19, 6), new Vector2Int(16, 7), 
            new Vector2Int(17, 7), new Vector2Int(18, 7), new Vector2Int(16, 8)
        };
        List<Vector2Int> coconut_positions = new List<Vector2Int>(){
            new Vector2Int(-4, 7), new Vector2Int(-5, 8), new Vector2Int(-3, 8), new Vector2Int(-2, 8), new Vector2Int(-2, 9)
        };

        var resourcePositions = new Dictionary<ResourceType, List<Vector2Int>>(){
            {ResourceType.Starfish, starfish_positions},
            {ResourceType.Grass, grass_positions},
            {ResourceType.Conch, conch_positions},
            {ResourceType.Coconut, coconut_positions}
        };

        // TODO: hard code
        var resList = new List<ResourceType>() {ResourceType.Starfish, ResourceType.Grass, ResourceType.Conch, ResourceType.Coconut};

        foreach (ResourceType type in resList) {
            List<Vector2Int> pos = resourcePositions[type];
            dictResourceTiles.Add(type, new List<ResourceTile>());
            foreach (Vector2Int ele in pos){
                Tile tile = GetResourceData(type).tile;
                // TODO: hard code
                ResourceTile resourceTile = new ResourceTile(type, tile, ele, 10);
                tilemapResource.SetTile((Vector3Int)resourceTile.position, resourceTile.tile);
                dictResourceTiles[type].Add(resourceTile);
            }
        }
    }

    void UpdateUI(){
        var resList = new List<ResourceType>() {ResourceType.Starfish, ResourceType.Grass, ResourceType.Conch, ResourceType.Coconut};
        foreach (ResourceType type in resList){
            //ui.AddResourceUI(type, info.sprite, type.ToString());
        }
    }

    public ResourceData GetResourceData(ResourceType type) {
        return dictResourceData[type];
    }
    public List<ResourceTile> GetResourceTiles(ResourceType type){
        return dictResourceTiles[type];
    }
}
