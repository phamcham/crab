using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_ResourceControl : UIE_UIControl {
    [Header("Avatar settings")]
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] Image avatar;

    [Space, Header("Control settings")]
    [SerializeField] Image resourceImg;
    [SerializeField] TMPro.TextMeshProUGUI amountText;
    [SerializeField] TMPro.TextMeshProUGUI gatheringTimeText;
    [SerializeField] HealthBarUI progressBarUI;

    Resource resource;
    public void SetResource(Resource resource) {
        this.resource = resource;
    }

    protected override void UpdateIntervalOnUI() {
        titleText.SetText(resource.properties.nameResource);
        avatar.sprite = resourceImg.sprite = resource.GetSprite();
        amountText.SetText("{0}", resource.properties.amount);
        gatheringTimeText.SetText("{0}", resource.properties.gatheringTime);

        progressBarUI.SetSize(1.0f - resource.properties.currentGatheringTime / resource.properties.gatheringTime);
    }
}
