using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSlotData {
    // map state
    public List<SaveSystemExtension.Vector2Int> grounds;
    public List<SaveSystemExtension.Vector2Int> waters;
    public List<ResourceSaveData> playerResources;
    // player building
    public HeadquarterBuildingSaveData playerHeadquarterBuilding;
    public List<HouseBuildingSaveData> playerHouseBuildings;
    public List<SandWallBuildingSaveData> playerSandWallBuildings;
    // enemy buildings
    public EnemyHeadquarterBuildingSaveData enemyHeadquarterBuilding;
    // player units
    public List<GatheringCrabUnitSaveData> playerGatheringCrabs;
    public List<BubbleCrabUnitSaveData> playerBubbleCrabs;
    public List<HermitCrabUnitSaveData> playerHermitCrabs;
    // enemy units
    public List<BubbleEnemyCrabUnitSaveData> enemyBubbleCrabs;

    // player stored resources
    //public List<
}
