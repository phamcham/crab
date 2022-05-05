using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_ResourceRequirement : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] UnityEvent<string> amountUpdateText;
    [SerializeField] Image amountBackgroundColor;
    [SerializeField] Color enoughColor;
    [SerializeField] Color notEnoughColor;
    public void Setup(ResourceRequirement requirement) {
        //image.sprite = requirement.data.tile.sprite;
        image.sprite = ResourceManager.current.GetResourceSprite(requirement.type);
        amountUpdateText?.Invoke(requirement.amount + "");

        requirement.enough = requirement.amount <= ResourceManager.current.GetAmount(requirement.type);

        if (requirement.enough) amountBackgroundColor.color = enoughColor;
        else amountBackgroundColor.color = notEnoughColor;
    }
}
