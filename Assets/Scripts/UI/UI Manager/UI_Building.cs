using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.UI;

public class UI_Building : MonoBehaviour {
    [SerializeField] private Transform buildingButtonsHolder;
    [SerializeField] UIE_CreateBuilding uieCreateBuildingPrefab;
    [SerializeField] UIE_ShowBuildingInfo uieShowBuildingInfo;
    Queue<GameObject> emptiers = new Queue<GameObject>();

    private void Awake() {
        buildingButtonsHolder.PCDestroyChildren();

        for (int i = 0; i < 5 * 3; i++){
            Transform trans = new GameObject().AddComponent<RectTransform>().transform;
            trans.gameObject.AddComponent<Image>().color = new Color(0.764151f, 0.7197218f, 0.4721876f);
            trans.SetParent(buildingButtonsHolder);
            emptiers.Enqueue(trans.gameObject);
        }
        
        uieShowBuildingInfo.gameObject.SetActive(false);
    }
    public void AddBuildingUI(Building building){
        UIE_CreateBuilding creater = Instantiate(uieCreateBuildingPrefab.gameObject, buildingButtonsHolder)
            .GetComponent<UIE_CreateBuilding>();
        creater.Setup(building, () => {
            string name = building.name;
            string description = building.description;
            List<ResourceRequirement> requirements = building.resourceRequirements;
            // BUG: thay vi setup, chuyen thanh update
            uieShowBuildingInfo.Setup(name, description, requirements);
            uieShowBuildingInfo.gameObject.SetActive(true);
        }, () => {
            uieShowBuildingInfo.gameObject.SetActive(false);
        });
        creater.transform.SetAsFirstSibling();
        
        if (emptiers.Count > 0){
            Destroy(emptiers.Dequeue());
        }
    }
}
