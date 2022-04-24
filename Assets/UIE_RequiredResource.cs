using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIE_RequiredResource : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] UnityEvent<string> amountUpdateText;
    public void UpdateUI(RequiredResource requiredResource){
        image.sprite = Resource  requiredResource.type
    }
}
