using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIE_SandWallBuildingControl : UIE_UIControl
{
    [Header("Avatar settings")]
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] Image avatar;
    [SerializeField] TMPro.TextMeshProUGUI healthPoint;
    [SerializeField] HealthBarUI healthBarUI;

    SandWallBuilding building;
    protected override void UpdateIntervalOnUI() {
        titleText.text = building.properties.buildingName;
        avatar.sprite = building.GetSprite();
        avatar.GetComponent<AspectRatioFitter>().aspectRatio = 1.0f * avatar.sprite.rect.width / avatar.sprite.rect.height;
        healthPoint.SetText("HP: {0}/{1}", building.properties.curHealthPoint, building.properties.maxHealthPoint);
        healthBarUI.SetSize(1.0f * building.properties.curHealthPoint / building.properties.maxHealthPoint);
    }

    public void SetBuilding(SandWallBuilding building) {
        this.building = building;
    }
}
