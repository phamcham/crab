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
    [SerializeField] UnityEvent<string> defendPointUpdateText;
    [SerializeField] UnityEvent<string> productionTimePointUpdateText;
    [SerializeField] UnityEvent<string> percentUpdateText;
    [SerializeField] Button continuePauseButton;
    [SerializeField] Image continuePauseButtonRender;
    [SerializeField] Sprite continueSprite;
    [SerializeField] Sprite pauseSprite;
    bool isContinue = true;
    HeadquarterBuilding building;
    float updateInterval = 0.2f;
    float curUpdateUIInterval;
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
        
        // control
        defendPointUpdateText?.Invoke(properties.defendPoint + "");
        productionTimePointUpdateText?.Invoke(ownProperties.productionInterval + "");
        percentUpdateText?.Invoke(ownProperties.curProductionPercent + "%");
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
