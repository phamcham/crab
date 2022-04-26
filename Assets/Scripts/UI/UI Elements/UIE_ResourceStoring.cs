using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_ResourceStoring : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] UnityEvent<string> amountUpdateText;
    int amount;
    public void Setup(ResourceType type, int add) {
        //image.sprite = requirement.data.tile.sprite;
        image.sprite = ResourceManager.current.GetResourceData(type).tile.sprite;
        amountUpdateText?.Invoke((amount + add) + "");
    }
}
