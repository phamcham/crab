 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_HeadquarterBuildingControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] Image avatar;
    [SerializeField] TMPro.TextMeshProUGUI healthPointText;
    [SerializeField] HealthBarUI healthBarUI;
    [SerializeField] TMPro.TextMeshProUGUI productionTimeText;
    [SerializeField] TMPro.TextMeshProUGUI percentText;
    [SerializeField] HealthBarUI spawnCrabProcessBarUI;
    [SerializeField] Color enoughBarColor;
    [SerializeField] Color notEnoughBarColor;
    [SerializeField] Button continuePauseButton;
    [SerializeField] Image continuePauseButtonRender;
    [SerializeField] Sprite continueSprite;
    [SerializeField] Sprite pauseSprite;
    bool isContinue = true;
    HeadquarterBuilding building;
    bool enoughCapacity = true;
    protected override void Start() {
        base.Start();
        continuePauseButton.onClick.AddListener(() => {
            if (isContinue) {
                continuePauseButtonRender.sprite = continueSprite;
                building.PauseProduction();
                isContinue = false;
            }
            else {
                continuePauseButtonRender.sprite = pauseSprite;
                building.ContinueProduction();
                isContinue = true;
            }
            Debug.Log("setcon: " + isContinue);
        });
        continuePauseButtonRender.sprite = pauseSprite;
        building.ContinueProduction();
    }
    public void SetBuilding(HeadquarterBuilding building) {
        this.building = building;
    }
    protected override void UpdateIntervalOnUI() {
        BuildingProperties properties = building.properties;
        HeadquarterBuilding.OwnProperties ownProperties = building.ownProperties;
        // avatar
        titleText.SetText(properties.buildingName);
        avatar.sprite = building.GetSprite();
        healthPointText.SetText("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint);
        healthBarUI.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        
        // control
        productionTimeText.SetText("{0}", ownProperties.productionInterval);
        if (enoughCapacity) percentText.SetText("{0}%", ownProperties.curProductionPercent);
        spawnCrabProcessBarUI.SetSize(1.0f * ownProperties.curProductionPercent / 100);
    }
    public void EnoughCapacityForCrab(bool enough) {
        enoughCapacity = enough;
        if (enough) {
            spawnCrabProcessBarUI.SetColor(enoughBarColor);
        }
        else {
            percentText.SetText("khong du nha roi");
            spawnCrabProcessBarUI.SetColor(notEnoughBarColor);
        }
    }
}
