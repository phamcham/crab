using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_HeadquarterBuildingControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] UnityEvent<string> titleUpdateText;
    [SerializeField] Image avatar;
    [SerializeField] UnityEvent<string> healthPoint;
    //[Space, Header("Control settings")]
    public void Setup(HeadquarterBuilding building) {
        BuildingProperties properties = building.properties;
        // avatar
        titleUpdateText?.Invoke(properties.buildingName);
        avatar.sprite = building.GetSprite();
        healthPoint?.Invoke(string.Format("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint));
        
        // control
    }
}
