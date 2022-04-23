using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController current { get; private set; }
    Dictionary<ResourceType, int> storedResources = new Dictionary<ResourceType, int>();
    HeadquarterBuilding headquarterBuilding;
    private void Awake() {
        current = this;
    }

    public void HeadquarterStartGameplay(HeadquarterBuilding headquarter){
        headquarterBuilding = headquarter;
    }
    public HeadquarterBuilding GetHeadquarterBuilding(){
        return headquarterBuilding;
    }
}
