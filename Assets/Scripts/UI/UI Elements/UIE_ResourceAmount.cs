using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIE_ResourceAmount : MonoBehaviour
{
    [SerializeField] Image resourceImage;
    [SerializeField] Text amountText;
    ResourceType resourceType;
    int amount;
    public void Setup(ResourceType resourceType, Sprite sprite, string name, int amount = 0){
        resourceImage.sprite = sprite;
        this.amount = amount;
        amountText.text = amount + "";
        this.resourceType = resourceType;
    }
    public void UpdateAmount(int amount){
        this.amount = amount;
        amountText.text = this.amount + "";
    }
    public void AddAmount(int add){
        this.amount += add;
        amountText.text = this.amount + "";
    }
}
