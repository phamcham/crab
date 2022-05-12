using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_GatheringCrabUnitControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] Image avatar;
    [SerializeField] TMPro.TextMeshProUGUI healthPointText;
    [SerializeField] HealthBarUI healthBarUI;
    
    [Space, Header("Control settings")]
    [SerializeField] TMPro.TextMeshProUGUI speedText;
    [SerializeField] TMPro.TextMeshProUGUI damageText;
    [SerializeField] Button hermitUpgradeButton;
    [SerializeField] Button bubbleUpgradeButton;
    GatheringCrabUnit unit;
    public void SetUnit(GatheringCrabUnit unit) {
        this.unit = unit;

        hermitUpgradeButton.onClick.RemoveListener(HermitUpgrade);
        bubbleUpgradeButton.onClick.RemoveListener(BubbleUpgrade);

        hermitUpgradeButton.onClick.AddListener(HermitUpgrade);
        bubbleUpgradeButton.onClick.AddListener(BubbleUpgrade);
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

    private void HermitUpgrade() {
        int conch = ResourceManager.current.GetAmount(ResourceType.Conch);
        if (conch > 0) {
            this.unit.HermitUpgrade();
        }
    }
    private void BubbleUpgrade() {
        int coconut = ResourceManager.current.GetAmount(ResourceType.Coconut);
        if (coconut > 0) {
            this.unit.BubbleUpgrade();
        }
    }
}
