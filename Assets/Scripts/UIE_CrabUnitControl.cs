using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_CrabUnitControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] UnityEvent<string> titleUpdateText;
    [SerializeField] Image avatar;
    [SerializeField] UnityEvent<string> healthPoint;
    
    [Space, Header("Control settings")]
    [SerializeField] UnityEvent<string> speedUpdateText;
    [SerializeField] UnityEvent<string> damageUpdateText;
    public void Setup(CrabUnit unit) {
        UnitProperties properties = unit.properties;
        // avatar
        titleUpdateText?.Invoke(properties.unitName);
        avatar.sprite = unit.GetSprite();
        healthPoint?.Invoke(string.Format("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint));
        
        // control
        speedUpdateText?.Invoke(properties.moveSpeed + "");
        damageUpdateText?.Invoke(properties.damage + "");
    }
}
