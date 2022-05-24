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
    [Header("Buttons")]
    [SerializeField] Button spawnGatheringCrabButton;
    [SerializeField] TMPro.TextMeshProUGUI spawnGatheringCrabPercentText;
    [SerializeField] HealthBarUI spawnGatheringCrabProgressBar;
    [SerializeField] Button spawnBubbleCrabButton;
    [SerializeField] TMPro.TextMeshProUGUI spawnBubbleCrabPercentText;
    [SerializeField] HealthBarUI spawnBubbleCrabProgressBar;
    [SerializeField] Button spawnHermitCrabButton;
    [SerializeField] TMPro.TextMeshProUGUI spawnHermitCrabPercentText;
    [SerializeField] HealthBarUI spawnHermitProgressBar;
    HeadquarterBuilding building;
    protected override void Start() {
        base.Start();
        spawnGatheringCrabButton.onClick.AddListener(() => building.AddProductionQueue(UnitType.Gathering));
        spawnBubbleCrabButton.onClick.AddListener(() => building.AddProductionQueue(UnitType.Bubble));
        spawnHermitCrabButton.onClick.AddListener(() => building.AddProductionQueue(UnitType.Hermit));
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
        UpdateCrabButtonUI(0, spawnGatheringCrabPercentText, spawnGatheringCrabProgressBar);
        UpdateCrabButtonUI(0, spawnBubbleCrabPercentText, spawnBubbleCrabProgressBar);
        UpdateCrabButtonUI(0, spawnHermitCrabPercentText, spawnHermitProgressBar);

        float percentNormalize = ownProperties.curProductionTime / ownProperties.productionInterval;
        if (building.ownProperties.curUnitType == UnitType.Gathering) {
            UpdateCrabButtonUI(percentNormalize, spawnGatheringCrabPercentText, spawnGatheringCrabProgressBar);
        }
        if (building.ownProperties.curUnitType == UnitType.Bubble) {
            UpdateCrabButtonUI(percentNormalize, spawnBubbleCrabPercentText, spawnBubbleCrabProgressBar);
        }
        if (building.ownProperties.curUnitType == UnitType.Hermit) {
            UpdateCrabButtonUI(percentNormalize, spawnHermitCrabPercentText, spawnHermitProgressBar);
        }
        //print(building.ownProperties.curUnitProductionName);
    }

    private void UpdateCrabButtonUI(float percentNormalize, TMPro.TextMeshProUGUI percentText, HealthBarUI progressBar) {
        if (percentNormalize > 0) {
            percentText.gameObject.SetActive(true);
            percentText.SetText((int) (100 * percentNormalize) + "%");
            progressBar.gameObject.SetActive(true);
            progressBar.SetSize(percentNormalize);
        }
        else {
            percentText.gameObject.SetActive(false);
            progressBar.gameObject.SetActive(false);
        }
    }
}
