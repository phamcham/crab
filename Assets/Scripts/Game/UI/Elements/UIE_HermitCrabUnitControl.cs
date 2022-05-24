using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_HermitCrabUnitControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] Image avatar;
    [SerializeField] TMPro.TextMeshProUGUI healthPointText;
    [SerializeField] HealthBarUI healthBarUI;
    
    [Space, Header("Control settings")]
    [SerializeField] TMPro.TextMeshProUGUI speedText;
    [SerializeField] TMPro.TextMeshProUGUI damageText;
    HermitCrabUnit unit;
    public void SetUnit(HermitCrabUnit unit) {
        this.unit = unit;
    }

    protected override void UpdateIntervalOnUI() {
        UnitProperties properties = unit.properties;
        // avatar
        titleText.SetText(properties.unitName);
        avatar.sprite = unit.GetSprite();
        healthPointText.SetText("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint);
        healthBarUI.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        // control
        speedText.SetText("{0}", properties.moveSpeed);
        damageText.SetText("{0}", properties.damage);
    }
}
