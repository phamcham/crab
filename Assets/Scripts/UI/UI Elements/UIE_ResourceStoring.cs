using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_ResourceStoring : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] UnityEvent<string> amountUpdateText;
    public void Setup(ResourceType type, int amount) {
        //image.sprite = requirement.data.tile.sprite;
        image.sprite = ResourceManager.current.GetResourceSprite(type);
        amountUpdateText?.Invoke(amount + "");
    }
}
