using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_LawnSprinklerBuildingControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] Image avatar;
    [SerializeField] TMPro.TextMeshProUGUI healthPointText;
    [SerializeField] HealthBarUI healthBarUI;
    [Header("Control")]
    [SerializeField] TMPro.TextMeshProUGUI attackRangeText;
    [SerializeField] TMPro.TextMeshProUGUI bulletSpeedText;
    [SerializeField] TMPro.TextMeshProUGUI bulletReloadTimeText;
    [SerializeField] TMPro.TextMeshProUGUI damageText;
    LawnSprinklerBuilding building;
    public void SetBuilding(LawnSprinklerBuilding building) {
        this.building = building;
    }

    protected override void UpdateIntervalOnUI() {
        BuildingProperties properties = building.properties;
        LawnSprinklerBuilding.OwnProperties ownProperties = building.ownProperties;
        // avatar
        titleText.SetText(properties.buildingName);
        avatar.sprite = building.GetSprite();
        healthPointText.SetText("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint);
        healthBarUI.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        // control
        attackRangeText.SetText("{0}", ownProperties.attackRadius);
        bulletReloadTimeText.SetText("{0}", ownProperties.reloadingTime);
        bulletSpeedText.SetText("{0}", ownProperties.bulletSpeed);
        damageText.SetText("{0}", ownProperties.damage);
    }
}
