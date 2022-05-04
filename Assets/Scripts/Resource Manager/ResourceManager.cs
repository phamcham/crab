using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager current { get; private set; }
    [SerializeField] Tilemap tilemapGround;
    [SerializeField] List<Resource> resourcePrefabs;
    [SerializeField] UI_Resource uiResource;
    [SerializeField] UI_Building uiBuilding;
    Dictionary<ResourceType, Resource> dictResourcePrefabs = new Dictionary<ResourceType, Resource>();
    Dictionary<ResourceType, List<Resource>> dictResources = new Dictionary<ResourceType, List<Resource>>();
    Dictionary<ResourceType, int> resourceStorage = new Dictionary<ResourceType, int>();
    Transform holder;
    private void Awake() {
        current = this;
        
        holder = new GameObject("Resource Holder").transform;
        holder.position = Vector3.zero;
        
        InitializeResourceDataDict();
    }

    private void Start() {
        FillResourcesToTilemap();
    }

    void InitializeResourceDataDict(){
        foreach (Resource resourcePrefab in resourcePrefabs){
            dictResourcePrefabs.Add(resourcePrefab.GetResourceType(), resourcePrefab);
            resourceStorage.Add(resourcePrefab.GetResourceType(), 0);
        }
    }

    void FillResourcesToTilemap() {

        List<Vector2Int> availablePositions = new List<Vector2Int>();
        foreach (Vector2Int pos in tilemapGround.cellBounds.allPositionsWithin){
            TileBase tile = tilemapGround.GetTile((Vector3Int)pos);
            if (tile != null){
                availablePositions.Add(pos);
            }
        }
        availablePositions.PCShuffer();
        Stack<Vector2Int> stack = new Stack<Vector2Int>(availablePositions);
        Dictionary<ResourceType, List<Vector2Int>> resourcePositions = new Dictionary<ResourceType, List<Vector2Int>>(){
            { ResourceType.Starfish, new List<Vector2Int>() },
            { ResourceType.Grass, new List<Vector2Int>() },
            { ResourceType.Conch, new List<Vector2Int>() },
            { ResourceType.Coconut, new List<Vector2Int>() }
        };
        foreach (ResourceType type in resourcePositions.Keys){
            for (int i = 0; i < 5; i++){
                if (stack.Count > 0) {
                    Vector2Int pos = stack.Pop();
                    resourcePositions[type].Add(pos);
                }
            }
        }

        // TODO: hard code
        var resList = new List<ResourceType>() {ResourceType.Starfish, ResourceType.Grass, ResourceType.Conch, ResourceType.Coconut};

        foreach (ResourceType type in resList) {
            List<Vector2Int> pos = resourcePositions[type];
            dictResources.Add(type, new List<Resource>());
            foreach (Vector2Int ele in pos){
                // TODO: hard code
                Resource resource = Create(type);
                resource.transform.position = (Vector2)ele + new Vector2(0.5f, 0.5f);
                dictResources[type].Add(resource);
            }
        }
    }
    public Sprite GetResourceSprite(ResourceType type){
        return dictResourcePrefabs[type].GetSprite();
    }
    // get resources is placing on map
    public List<Resource> GetResources(ResourceType type){
        if (type == ResourceType.None) return null;
        return dictResources[type];
    }
    public List<ResourceType> GetResourceTypes(){
        return new List<ResourceType>(dictResources.Keys);
    }
    public void CollectResource(ResourceType type, int add){
        resourceStorage[type] += add;
        uiResource.UpdateResourceStoringUI(type, resourceStorage[type]);
        uiBuilding.UpdateCreateBuildingUI();
    }
    public int GetAmount(ResourceType type) {
        return resourceStorage[type];
    }
    public Resource Create(ResourceType type) {
        return Instantiate(dictResourcePrefabs[type], holder);
    }
}
