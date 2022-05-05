using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_HouseBuildingControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] Image avatar;
    [SerializeField] TMPro.TextMeshProUGUI healthPointText;
    [SerializeField] HealthBarUI healthBarUI;
    [SerializeField] TMPro.TextMeshProUGUI capacityPointText;
    HouseBuilding building;

    public void SetBuilding(HouseBuilding building) {
        //print("assigned?" +(building != null ? "ok" : "null"));
        this.building = building;
    }

    protected override void UpdateIntervalOnUI() {
        //print("building null: " + (building is null ? "null" : "ok"));
        BuildingProperties properties = building.properties;
        HouseBuilding.OwnProperties ownProperties = building.ownProperties;
        // avatar
        titleText.SetText(properties.buildingName);
        avatar.sprite = building.GetSprite();
        healthPointText.SetText("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint);
        healthBarUI.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        // control
        capacityPointText.SetText("{0}", ownProperties.capacity);
    }
}
