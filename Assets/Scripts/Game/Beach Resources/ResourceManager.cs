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
    private void Awake() {
        current = this;
        
        InitializeResourceDataDict();
    }

    private void Start() {
        FillResourcesToTilemap();
    }

    void InitializeResourceDataDict(){
        foreach (Resource resourcePrefab in resourcePrefabs){
            dictResourcePrefabs.Add(resourcePrefab.properties.type, resourcePrefab);
            resourceStorage.Add(resourcePrefab.properties.type, 0);
        }
    }

    void FillResourcesToTilemap() {

        List<Vector2Int> availablePositions = new List<Vector2Int>();
        foreach (Vector2Int pos in tilemapGround.cellBounds.allPositionsWithin){
            TileBase tile = tilemapGround.GetTile((Vector3Int)pos);
            if (tile != null) {
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
            for (int i = 0; i < 4; i++){
                if (stack.Count > 0) {
                    Vector2Int pos = stack.Pop();
                    resourcePositions[type].Add(pos);
                }
            }
        }

        // TODO: hard code
        var resList = new List<ResourceType>() {ResourceType.Starfish, ResourceType.Grass, ResourceType.Conch, ResourceType.Coconut};

        HashSet<Vector2> set = new HashSet<Vector2>();
        foreach (ResourceType type in resList) {
            List<Vector2Int> listPos = resourcePositions[type];
            dictResources.Add(type, new List<Resource>());
            foreach (Vector2Int pos in listPos) {
                // TODO: hard code
                if (set.Contains(pos)) {
                    continue;
                }
                int n = Random.Range(4, 7);
                for (int i = 0; i < n; i++) {
                    Vector2Int randomPos = Vector2Int.RoundToInt(pos + Random.insideUnitCircle * 3);

                    TileBase tile = tilemapGround.GetTile((Vector3Int)randomPos);
                    if (tile != null) {
                        Resource resource = Create(type);
                        resource.SetAmount(Random.Range(20, 40));
                        resource.transform.position = randomPos + new Vector2(0.5f, 0.5f);
                        dictResources[type].Add(resource);
                        set.Add(randomPos);
                    }
                }
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
    public List<Resource> GetAllResources() {
        List<Resource> resources = new List<Resource>();
        
        List<ResourceType> resourceTypes = ResourceManager.current.GetResourceTypes();
        foreach (ResourceType type in resourceTypes) {
            foreach (Resource resource in ResourceManager.current.GetResources(type)) {
                resources.Add(resource);
            }
        }
        return resources;
    }
    public void DeltaResource(ResourceType type, int add){
        resourceStorage[type] += add;
        uiResource.UpdateResourceStoringUI(type, resourceStorage[type]);
        uiBuilding.UpdateCreateBuildingUI();
    }
    public int GetAmount(ResourceType type) {
        return resourceStorage[type];
    }
    public Resource Create(ResourceType type) {
        return Instantiate(dictResourcePrefabs[type], transform);
    }
}
