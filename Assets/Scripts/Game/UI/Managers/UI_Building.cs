using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.UI;

public class UI_Building : MonoBehaviour {
    [SerializeField] private Transform buildingButtonsHolder;
    [SerializeField] UIE_CreateBuilding uieCreateBuildingPrefab;
    [SerializeField] UIE_ShowBuildingInfo uieShowBuildingInfo;
    Stack<GameObject> space = new Stack<GameObject>();
    Stack<GameObject> deactiveSpace = new Stack<GameObject>();
    List<UIE_CreateBuilding> createBuildings = new List<UIE_CreateBuilding>();

    private void Awake() {
        buildingButtonsHolder.PCDestroyChildren();

        for (int i = 0; i < 5 * 2; i++) {
            GameObject spacerObj = new GameObject("spacer");
            spacerObj.AddComponent<RectTransform>();
            spacerObj.AddComponent<Image>().color = new Color(0.764151f, 0.7197218f, 0.4721876f);
            spacerObj.transform.SetParent(buildingButtonsHolder);
            space.Push(spacerObj);
        }
        
        uieShowBuildingInfo.gameObject.SetActive(false);
    }
    public void AddBuildingUI(Building building){
        UIE_CreateBuilding creater = Instantiate(uieCreateBuildingPrefab.gameObject, buildingButtonsHolder)
            .GetComponent<UIE_CreateBuilding>();
        creater.Setup(building, () => {
            string name = building.properties.buildingName;
            string description = building.properties.description;
            List<ResourceRequirement> requirements = building.properties.resourceRequirements;
            // BUG: thay vi setup, chuyen thanh update
            uieShowBuildingInfo.Setup(name, description, requirements);
            uieShowBuildingInfo.gameObject.SetActive(true);
        }, () => {
            uieShowBuildingInfo.gameObject.SetActive(false);
        });
        creater.transform.SetAsFirstSibling();
        createBuildings.Add(creater);
        
        if (space.Count > 0){
            //Destroy(space.Dequeue());
            GameObject obj = space.Pop();
            obj.SetActive(false);
            deactiveSpace.Push(obj);
        }
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
            space.Push(obj);
        }
    }

    public void UpdateCreateBuildingUI() {
        foreach (UIE_CreateBuilding creater in createBuildings) {
            creater.UpdateUI();
        }
    }
}
