using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {
    public static GameController current { get; private set; }
    public bool IsGameplayPaused { get; private set; }
    public bool IsApplicationQuited { get; private set; }
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] Tilemap waterTilemap;
    [SerializeField] UIE_PauseGame uiePauseGame;
    //
    [SerializeField] Tile groundTile;
    [SerializeField] Tile waterTile;
    [SerializeField] Tile borderTile;
    private void Awake() {
        current = this;
    }

    private void Start() {
        InitGame();
        ResumeGame();
    }

    public void Load() {
        
    }

    void InitGame() {
        ResourceManager.current.DeltaResource(ResourceType.Coconut, 1000);
        ResourceManager.current.DeltaResource(ResourceType.Conch, 1000);
        ResourceManager.current.DeltaResource(ResourceType.Grass, 1000);
        ResourceManager.current.DeltaResource(ResourceType.Starfish, 1000);

        //Genmap();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (IsGameplayPaused) {
                ResumeGame();
            }
            else {
                PauseGame();
            }
        }
    }
    public void PauseGame() {
        IsGameplayPaused = true;
        uiePauseGame.OpenPauseWindowUI();
        Time.timeScale = 0;
    }
    public void ResumeGame() {
        IsGameplayPaused = false;
        uiePauseGame.ClosePauseWindowUI();
        Time.timeScale = 1;
    }

    public void OnMainMenu() {
        SceneManager.LoadScene("StartScene");
    }

    private void OnApplicationQuit() {
        IsApplicationQuited = false;
    }

    public void SaveCurrentMap() {
        GameSlotData gameData = new GameSlotData() {
            grounds = GetTilemapCells(groundTilemap),
            waters = GetTilemapCells(waterTilemap),
            playerResources = ResourceManager.current.GetAllResources().Select(x => x.GetSaveObjectData()).ToList(),
            playerHeadquarterBuilding = BuildingManager.current.GetHeadquarterBuilding().GetSaveObjectData(),
            playerHouseBuildings = BuildingManager.current.HouseBuildings.Select(x => x.GetSaveObjectData()).ToList(),
            playerSandWallBuildings = BuildingManager.current.SandWallBuildings.Select(x => x.GetSaveObjectData()).ToList(),
            // enemey o day
            playerGatheringCrabs = UnitManager.current.GatheringCrabUnits.Select(x => x.GetSaveObjectData()).ToList(),
            playerBubbleCrabs = UnitManager.current.BubbleCrabUnits.Select(x => x.GetSaveObjectData()).ToList(),
            playerHermitCrabs = UnitManager.current.HermitCrabUnits.Select(x => x.GetSaveObjectData()).ToList(),
            // enemy crab o day
        };

        SaveSystem.SaveJson(gameData, Random.Range(1000000, 9999999) + ".txt");
    }

    private List<SaveSystemExtension.Vector2Int> GetTilemapCells(Tilemap tilemap) {
        List<SaveSystemExtension.Vector2Int> grounds = new List<SaveSystemExtension.Vector2Int>();
        foreach (Vector2Int cell in tilemap.cellBounds.allPositionsWithin) {
            if (tilemap.GetTile((Vector3Int)cell) != null) {
                grounds.Add(new SaveSystemExtension.Vector2Int(cell));
            }
        }
        return grounds;
    }
}
