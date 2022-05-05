using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_NumberOfThings : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] UnityEvent<string> amountUpdateText;
    public void Setup(Sprite sprite, int amount) {
        image.sprite = sprite;
        amountUpdateText?.Invoke(amount + "");
    }
}
