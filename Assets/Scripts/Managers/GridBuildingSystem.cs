using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridBuildingSystem : MonoBehaviour {
    public static GridBuildingSystem current {get; private set;}

    public GridLayout gridLayout;
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] TileBase walkableTile;
    [SerializeField] Tilemap mainTilemap;
    [SerializeField] Tilemap tempTilemap;
    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    private GameObject tempObj; // only sprite
    private Building tempBuilding;

    private Vector3Int prevPos;
    private BoundsInt prevArea;
    Transform holder;
    
    // call neu co thay doi o map
    public void UpdateMainTilemap(){
        mainTilemap.ClearAllTiles();
        BoundsInt groundArea = groundTilemap.cellBounds;
        List<Vector3Int> positionList = new List<Vector3Int>();

        foreach (Vector3Int v in groundArea.allPositionsWithin){
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            TileBase tile = groundTilemap.GetTile(pos);
            if (tile != null && tile == walkableTile){
                positionList.Add(pos);
            }
        }

        TileBase[] tileArray = new TileBase[positionList.Count];
        FillTiles(tileArray, TileType.White);

        mainTilemap.SetTiles(positionList.ToArray(), tileArray);
    }
    // TODO: nen sua thanh Create cho dong bo
    public void InitializeWithBuilding(Building building){
        tempBuilding = Instantiate(building.gameObject, holder).GetComponent<Building>();
        tempBuilding.gameObject.SetActive(false);

        tempObj = new GameObject(tempBuilding.properties.buildingName + " temp");
        SpriteRenderer spriteRenderer = tempObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = building.GetSprite();
        spriteRenderer.sortingOrder = 3;
        tempObj.transform.position = InputExtension.MouseWorldPoint();

        FollowBuilding();
        SetActiveBuilding(true);
    }
    private void ClearArea(){
        int prevAreaSize = prevArea.size.x * prevArea.size.y * prevArea.size.z;
        TileBase[] toClear = new TileBase[prevAreaSize];
        FillTiles(toClear, TileType.Empty);
        tempTilemap.SetTilesBlock(prevArea, toClear);
    }
    private void FollowBuilding(){
        ClearArea();

        BoundsInt buildingArea = CalculateAreaFromWorldPosition(tempBuilding.properties.area, tempBuilding.transform.position);
        tempBuilding.properties.area.position = buildingArea.position;

        TileBase[] baseArray = GetTilesBlock(buildingArea, mainTilemap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];
        for (int i = 0; i < size; i++){
            if (baseArray[i] == tileBases[TileType.White]){
                tileArray[i] = tileBases[TileType.Green];
            }
            else{
                FillTiles(tileArray, TileType.Red);
                break;
            }
        }

        tempTilemap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }
    public void TakeArea(BoundsInt area){
        SetTileBlock(area, TileType.Empty, tempTilemap);
        SetTileBlock(area, TileType.Green, mainTilemap);

        for (int i = 0; i < area.size.x; i++){
            for (int j = 0; j < area.size.y; j++){
                Vector2Int curPosition = new Vector2Int(i, j);
                Vector2Int cellIndex = AStarGrid.current.PositionToIndex(curPosition + (Vector2Int)area.position);
                //Debug.Log(cellIndex);
                AStarGrid.current.AddTempObstacle(cellIndex);
                AStarGrid.current.UpdateGridNode(cellIndex);
            }
        }
    }
    public void ClearArea(BoundsInt area) {
        SetTileBlock(area, TileType.White, mainTilemap);

        for (int i = 0; i < area.size.x; i++){
            for (int j = 0; j < area.size.y; j++){
                Vector2Int curPosition = new Vector2Int(i, j);
                Vector2Int cellIndex = AStarGrid.current.PositionToIndex(curPosition + (Vector2Int)area.position);
                //Debug.Log(cellIndex);
                AStarGrid.current.DeleteTempObstacle(cellIndex);
                AStarGrid.current.UpdateGridNode(cellIndex);
            }
        }
    }
    public bool IsBuilding(TileBase tile){
        print("Check: " + (tile == tileBases[TileType.Green]));
        return tile == tileBases[TileType.Green];
    }

    public BoundsInt CalculateAreaFromWorldPosition(BoundsInt area, Vector2 worldPosition){
        Vector3Int posInt = gridLayout.WorldToCell(worldPosition);
        BoundsInt areaTemp = area;
        Vector3Int offset = (areaTemp.size - Vector3Int.one) / 2;
        offset.z = 0;
        areaTemp.position = posInt - offset;
        return areaTemp;
    }
    private void Awake() {
        current = this;

        holder = new GameObject("Building Holder").transform;
        holder.position = Vector3.zero;
    }
    private void Start() {
        string tilePath = @"Tiles/";
        tileBases.Add(TileType.Empty, null);
        tileBases.Add(TileType.White, Resources.Load<TileBase>(tilePath + "white"));
        tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "green"));
        tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "red"));

        UpdateMainTilemap();
        SetActiveBuilding(false);
    }

    private void Update() {
        if (!tempObj){
            return;
        }
        if (!tempBuilding.IsPlaced){
            if (InputExtension.IsMouseOverUI()){
                return;
            }
            Vector2 touchPos = InputExtension.MouseWorldPoint();
            Vector3Int cellPos = gridLayout.WorldToCell(touchPos);

            tempBuilding.transform.position = touchPos;
            tempObj.transform.position = touchPos;
            if (prevPos != cellPos){
                prevPos = cellPos;
                FollowBuilding();
            }
            
            if (Input.GetMouseButtonDown(0)){
                if (tempBuilding.CanBePlaced()){
                    tempBuilding.gameObject.SetActive(true);
                    Destroy(tempObj);
                    tempObj = null;
                    tempBuilding.Place();
                    SetActiveBuilding(false);
                }
            }
            else if (Input.GetMouseButtonDown(1)){
                ClearArea();
                Destroy(tempBuilding.gameObject);
                Destroy(tempObj);
                tempBuilding = null;
                tempObj = null;
                SetActiveBuilding(false);
            }
        }
        
    }

    public bool CanTakeArea(BoundsInt area){
        TileBase[] baseArray = GetTilesBlock(area, mainTilemap);
        foreach (TileBase b in baseArray){
            if (b != tileBases[TileType.White]){
                return false;
            }
        }
        return true;
    }


    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap){
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] array = new TileBase[size];
        int counter = 0;
        foreach (Vector3Int v in area.allPositionsWithin){
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }
    private static void SetTileBlock(BoundsInt area, TileType type, Tilemap tilemap){
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }
    private static void FillTiles(TileBase[] arr, TileType type){
        for (int i = 0; i < arr.Length; i++){
            arr[i] = tileBases[type];
        }
    }

    private void SetActiveBuilding(bool active){
        mainTilemap.gameObject.SetActive(active);
        tempTilemap.gameObject.SetActive(active);
    }

    public enum TileType{
        Empty,
        White,
        Green,
        Red
    }
}
