using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIE_ResourcePrioritySetter : MonoBehaviour{
    [SerializeField] Image image;
    [SerializeField] Text text;
    public NumberSpinner numberSpinner;
    private ResourceType resourceType;
    public void Setup(ResourceType type, Sprite sprite, string name, int priority = 0){
        resourceType = type;
        image.sprite = sprite;
        text.text = name;
        numberSpinner.Value = priority;

        numberSpinner.OnValueSet.AddListener(OnEditEnd);
    }

    private void OnEditEnd(int priority){
        ResourcePrioritiesManager.current.UpdateResourcePriorities();
    }

    public int Priority => numberSpinner.Value;
    public ResourceType ResourceType => this.resourceType;
}
