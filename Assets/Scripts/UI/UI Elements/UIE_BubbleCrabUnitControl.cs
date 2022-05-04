using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_BubbleCrabUnitControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] UnityEvent<string> titleUpdateText;
    [SerializeField] Image avatar;
    [SerializeField] UnityEvent<string> healthPoint;
    [SerializeField] HealthBarUI healthBarUI;
    
    [Space, Header("Control settings")]
    [SerializeField] UnityEvent<string> speedUpdateText;
    [SerializeField] UnityEvent<string> damageUpdateText;
    [SerializeField] UnityEvent<string> attackRadiusUpdateText;
    [SerializeField] UnityEvent<string> bulletSpeedUpdateText;
    [SerializeField] UnityEvent<string> bulletReloadTimeUpdateText;

    UnitShootable shootable;
    public void SetUnitShootable(UnitShootable shootable) {
        this.shootable = shootable;
    }

    protected override void UpdateIntervalOnUI() {
        PlayerUnit unit = shootable.BaseUnit;
        UnitProperties properties = unit.properties;
        // avatar
        titleUpdateText?.Invoke(properties.unitName);
        avatar.sprite = unit.GetSprite();
        healthPoint?.Invoke(string.Format("HP: {0}/{1}", properties.curHealthPoint, properties.maxHealthPoint));
        healthBarUI.SetSize(1.0f * properties.curHealthPoint / properties.maxHealthPoint);
        // control
        speedUpdateText?.Invoke(properties.moveSpeed + "");
        damageUpdateText?.Invoke(properties.damage + "");
        attackRadiusUpdateText?.Invoke(shootable.attackRadius + "");
        bulletSpeedUpdateText?.Invoke(shootable.bulletSpeed + "");
        bulletReloadTimeUpdateText?.Invoke(shootable.reloadingTime + "");
    }
}
