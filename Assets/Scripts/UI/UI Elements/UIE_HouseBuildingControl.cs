using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_HouseBuildingControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] UnityEvent<string> titleUpdateText;
    [SerializeField] Image avatar;
    [SerializeField] UnityEvent<string> healthPointUpdateText;
    [SerializeField] HealthBarUI healthBarUI;
    [SerializeField] UnityEvent<string> capacityPointUpdateText;
    HouseBuilding building;
    float updateUIInterval = 0.2f;
    float curUpdateUITime;

    public void Setup(HouseBuilding building) {
        //print("assigned?" +(building != null ? "ok" : "null"));
        this.building = building;
    }
    void UpdateUI() {
        //print("building null: " + (building is null ? "null" : "ok"));
        BuildingProperties properties = building.properties;
        HouseBuilding.OwnProperties ownProperties = building.ownProperties;
        // avatar
        titleUpdateText?.Invoke(properties.buildingName);
        avatar.sprite = building.GetSprite();
        healthPointUpdateText?.Invoke(string.Format("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint));
        healthBarUI.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        // control
        capacityPointUpdateText?.Invoke(ownProperties.capacity + "");
    }
    private void OnEnable() {
        curUpdateUITime = 0;
    }
    private void Update() {
        if (curUpdateUITime <= 0) {
            curUpdateUITime = updateUIInterval;
            UpdateUI();
        }
        else {
            curUpdateUITime -= Time.deltaTime;
        }
    }
}
