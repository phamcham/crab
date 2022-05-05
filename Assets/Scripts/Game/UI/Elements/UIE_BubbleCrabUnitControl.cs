using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_BubbleCrabUnitControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] Image avatar;
    [SerializeField] TMPro.TextMeshProUGUI healthPoint;
    [SerializeField] HealthBarUI healthBarUI;
    
    [Space, Header("Control settings")]
    [SerializeField] TMPro.TextMeshProUGUI speedText;
    [SerializeField] TMPro.TextMeshProUGUI damageText;
    [SerializeField] TMPro.TextMeshProUGUI attackRadiusText;
    [SerializeField] TMPro.TextMeshProUGUI bulletSpeedText;
    [SerializeField] TMPro.TextMeshProUGUI bulletReloadTimeText;

    UnitTaskShooting shootable;
    public void SetUnitShootable(UnitTaskShooting shootable) {
        this.shootable = shootable;
    }

    protected override void UpdateIntervalOnUI() {
        PlayerUnit unit = shootable.BaseUnit;
        UnitProperties properties = unit.properties;
        // avatar
        titleText.SetText(properties.unitName);
        avatar.sprite = unit.GetSprite();
        healthPoint.SetText("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint);
        healthBarUI.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        // control
        speedText.SetText("{0}", properties.moveSpeed);
        damageText.SetText("{0}", properties.damage);
        attackRadiusText.SetText("{0}", shootable.attackRadius);
        bulletSpeedText.SetText("{0}", shootable.bulletSpeed);
        bulletReloadTimeText.SetText("{0}", shootable.reloadingTime);
    }
}
