 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_HeadquarterBuildingControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] UnityEvent<string> titleUpdateText;
    [SerializeField] Image avatar;
    [SerializeField] UnityEvent<string> healthPointUpdateText;
    [SerializeField] HealthBarUI healthBarUI;
    [SerializeField] UnityEvent<string> defendPointUpdateText;
    [SerializeField] UnityEvent<string> productionTimePointUpdateText;
    [SerializeField] UnityEvent<string> percentUpdateText;
    [SerializeField] HealthBarUI spawnCrabProcessBarUI;
    [SerializeField] Color enoughBarColor;
    [SerializeField] Color notEnoughBarColor;
    [SerializeField] Button continuePauseButton;
    [SerializeField] Image continuePauseButtonRender;
    [SerializeField] Sprite continueSprite;
    [SerializeField] Sprite pauseSprite;
    bool isContinue = true;
    HeadquarterBuilding building;
    float updateInterval = 0.2f;
    float curUpdateUIInterval;
    bool enoughCapacity = true;
    private void Start() {
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
    public void Setup(HeadquarterBuilding building) {
        this.building = building;
    }
    void UpdateUI() {
        BuildingProperties properties = building.properties;
        HeadquarterBuilding.OwnProperties ownProperties = building.ownProperties;
        // avatar
        titleUpdateText?.Invoke(properties.buildingName);
        avatar.sprite = building.GetSprite();
        healthPointUpdateText?.Invoke(string.Format("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint));
        healthBarUI.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        
        // control
        defendPointUpdateText?.Invoke(properties.defendPoint + "");
        productionTimePointUpdateText?.Invoke(ownProperties.productionInterval + "");
        if (enoughCapacity) percentUpdateText?.Invoke(ownProperties.curProductionPercent + "%");
        spawnCrabProcessBarUI.SetSize(1.0f * ownProperties.curProductionPercent / 100);
    }
    public void EnoughCapacityForCrab(bool enough) {
        enoughCapacity = enough;
        if (enough) {
            spawnCrabProcessBarUI.SetColor(enoughBarColor);
        }
        else {
            percentUpdateText?.Invoke("khong du nha roi");
            spawnCrabProcessBarUI.SetColor(notEnoughBarColor);
        }
    }
    private void OnEnable() {
        curUpdateUIInterval = 0;
    }
    private void Update() {
        if (curUpdateUIInterval <= 0) {
            curUpdateUIInterval = updateInterval;
            UpdateUI();
        }
        else {
            curUpdateUIInterval -= Time.deltaTime;
        }
    }
}
