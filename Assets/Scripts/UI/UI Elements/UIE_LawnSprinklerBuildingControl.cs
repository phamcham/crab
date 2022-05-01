using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_LawnSprinklerBuildingControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] UnityEvent<string> titleUpdateText;
    [SerializeField] Image avatar;
    [SerializeField] UnityEvent<string> healthPointUpdateText;
    [SerializeField] UnityEvent<string> attackRangeUpdateText;
    [SerializeField] UnityEvent<string> bulletSpeedUpdateText;
    [SerializeField] UnityEvent<string> bulletReloadTimeUpdateText;
    [SerializeField] UnityEvent<string> damageUpdateText;
    LawnSprinklerBuilding building;

    float updateUIInterval = 0.2f;
    float curUpdateUITime;
    public void Setup(LawnSprinklerBuilding building) {
        this.building = building;
    }
    void UpdateUI() {
        BuildingProperties properties = building.properties;
        LawnSprinklerBuilding.OwnProperties ownProperties = building.ownProperties;
        // avatar
        titleUpdateText?.Invoke(properties.buildingName);
        avatar.sprite = building.GetSprite();
        healthPointUpdateText?.Invoke(string.Format("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint));
        
        // control
        attackRangeUpdateText?.Invoke(ownProperties.attackRadius + "");
        bulletReloadTimeUpdateText?.Invoke(ownProperties.reloadingTime + "");
        bulletSpeedUpdateText?.Invoke(ownProperties.bulletSpeed + "");
        damageUpdateText?.Invoke(ownProperties.damage + "");
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
