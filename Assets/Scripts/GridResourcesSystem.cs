using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridResourcesSystem : MonoBehaviour
{
    public static GridResourcesSystem current { get; private set; }
    [SerializeField] Transform holder;
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] Resource resourcePrefab;
    [SerializeField] List<ResourceInfo> settings;
    Dictionary<ResourceType, List<Vector2Int>> resourcePositions = new Dictionary<ResourceType, List<Vector2Int>>();
    Dictionary<ResourceType, List<Resource>> resources = new Dictionary<ResourceType, List<Resource>>();
    private void Awake() {
        current = this;
    }

    private void Start() {
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
        resourcePositions = new Dictionary<ResourceType, List<Vector2Int>>(){
            {ResourceType.Starfish, starfish_positions},
            {ResourceType.Grass, grass_positions},
            {ResourceType.Conch, conch_positions},
            {ResourceType.Coconut, coconut_positions}
        };

        List<ResourceType> currentResources = GetCurrentResourcesAvailable();

        foreach (ResourceType type in currentResources) {
            List<Vector2Int> pos = resourcePositions[type];
            resources.Add(type, new List<Resource>());
            foreach (Vector2Int ele in pos){
                Resource res = Instantiate(resourcePrefab, holder).GetComponent<Resource>();
                ResourceInfo info = new ResourceInfo(settings.Find(x => x.type == type));
                info.area.position = new Vector3Int(ele.x, ele.y, 0);
                res.Setup(info, Random.Range(10, 20));
                resources[type].Add(res);
            }
        }

        foreach (ResourceType type in currentResources){
            ResourceInfo info = new ResourceInfo(settings.Find(x => x.type == type));
            UI_ResourcePriorities.current.AddSetterUI(type, info.sprite, type.ToString(), 0);
        }
        ResourcePrioritiesManager.current.UpdateResourcePriorities();
    }

    /*private void CheckResourcesAvailableOnTilemap(){
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
    }*/

    public List<Resource> GetResources(ResourceType type){
        if (resources.TryGetValue(type, out List<Resource> res)){
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