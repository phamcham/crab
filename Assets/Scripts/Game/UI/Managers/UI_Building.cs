using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.UI;

public class UI_Building : MonoBehaviour {
    [SerializeField] private Transform buildingButtonsHolder;
    [SerializeField] UIE_CreateBuilding uieCreateBuildingPrefab;
    [SerializeField] UIE_ShowBuildingInfo uieShowBuildingInfo;
    Stack<GameObject> deactiveSpace = new Stack<GameObject>();
    List<UIE_CreateBuilding> createBuildings = new List<UIE_CreateBuilding>();

    private void Awake() {
        buildingButtonsHolder.PCDestroyChildren();
    }
    private void Start() {
        uieShowBuildingInfo.gameObject.SetActive(false);
    }
    public void AddBuildingUI(Building building){
        UIE_CreateBuilding creater = Instantiate(uieCreateBuildingPrefab.gameObject, buildingButtonsHolder)
            .GetComponent<UIE_CreateBuilding>();
        
        KeyCode[] shortKey = new KeyCode[3] { KeyCode.E, KeyCode.W, KeyCode.Q };
        creater.Setup(building, () => {
            string name = building.properties.buildingName;
            string description = building.properties.description;
            List<ResourceRequirement> requirements = building.properties.resourceRequirements;
            // BUG: thay vi setup, chuyen thanh update
            uieShowBuildingInfo.Setup(name, description, requirements);
            uieShowBuildingInfo.gameObject.SetActive(true);
        }, () => {
            uieShowBuildingInfo.gameObject.SetActive(false);
        },
            shortKey[createBuildings.Count]
        );
        creater.transform.SetAsFirstSibling();
        createBuildings.Add(creater);
    
    }

    public void AddOnceHeadquarterBuildingUI(HeadquarterBuilding headquarter) {
        if (createBuildings.Count != 0) {
            Debug.LogError("Require Headquarter built first");
            return;
        }
        AddBuildingUI(headquarter);
    }

    public void RemoveOnceHeadquarterBuildingUI() {
        if (createBuildings.Count != 1) {
            Debug.LogError("Build Headquater first run into a problem");
            return;
        }
        Destroy(createBuildings[0].gameObject);
        createBuildings.Clear();
        if (deactiveSpace.Count > 0) {
            GameObject obj = deactiveSpace.Pop();
            obj.SetActive(true);
            obj.transform.SetAsLastSibling();
        }
    }

    public void UpdateCreateBuildingUI() {
        foreach (UIE_CreateBuilding creater in createBuildings) {
            creater.UpdateUI();
        }
    }
}
