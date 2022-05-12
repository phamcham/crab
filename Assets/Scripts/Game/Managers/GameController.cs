using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController current { get; private set; }
    public bool IsGameplayPaused { get; private set; }
    public bool IsApplicationQuited { get; private set; }
    [SerializeField] UIE_PauseGame uiePauseGame;
    Dictionary<ResourceType, int> storedResources = new Dictionary<ResourceType, int>();
    HeadquarterBuilding headquarterBuilding;
    private void Awake() {
        current = this;
    }

    private void Start() {
        InitGame();
        ResumeGame();
    }

    void InitGame() {
        ResourceManager.current.DeltaResource(ResourceType.Coconut, 1000);
        ResourceManager.current.DeltaResource(ResourceType.Conch, 1000);
        ResourceManager.current.DeltaResource(ResourceType.Grass, 1000);
        ResourceManager.current.DeltaResource(ResourceType.Starfish, 1000);
    }

    public void HeadquarterStartGameplay(HeadquarterBuilding headquarter){
        headquarterBuilding = headquarter;
        BuildingManager.current.RemoveHeadquarterUIAndAddOthersBuildingUI();
    }
    public HeadquarterBuilding GetHeadquarterBuilding(){
        return headquarterBuilding;
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

    private void OnApplicationQuit() {
        IsApplicationQuited = false;
    }
}
