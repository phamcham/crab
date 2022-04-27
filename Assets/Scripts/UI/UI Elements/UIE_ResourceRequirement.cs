using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_ResourceRequirement : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] UnityEvent<string> amountUpdateText;
    public void Setup(ResourceRequirement requirement) {
        //image.sprite = requirement.data.tile.sprite;
        image.sprite = ResourceManager.current.GetResourceSprite(requirement.type);
        amountUpdateText?.Invoke(requirement.amount + "");
    }
}
