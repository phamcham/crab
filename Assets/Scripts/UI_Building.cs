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
            trans.SetParent(buildingButtonsHolder);
            emptiers.Enqueue(trans.gameObject);
        }
        
        uieShowBuildingInfo.gameObject.SetActive(false);
    }
    public void AddBuildingUI(Building building){
        UIE_CreateBuilding creater = Instantiate(uieCreateBuildingPrefab.gameObject, buildingButtonsHolder)
            .GetComponent<UIE_CreateBuilding>();
        creater.Setup(building, OnHoverBuildingButton, OnExitBuildingButton);
        creater.transform.SetAsFirstSibling();

        if (emptiers.Count > 0){
            Destroy(emptiers.Dequeue());
        }
        Debug.Log(emptiers.Count);
    }
    private void OnHoverBuildingButton(Building.Info info){
        string name = info.name;
        string description = info.description;
        List<ResourceRequirement> requirements = info.resourceRequirements;

        uieShowBuildingInfo.gameObject.SetActive(true);
        // BUG: thay vi setup, chuyen thanh update
        uieShowBuildingInfo.Setup(name, description, requirements);
    }
    void OnExitBuildingButton(){
        uieShowBuildingInfo.gameObject.SetActive(false);
    }
}
