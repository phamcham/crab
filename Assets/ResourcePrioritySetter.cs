using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePrioritySetter : MonoBehaviour{
    [SerializeField] Image image;
    [SerializeField] Text text;
    [SerializeField] NumberSpinner numberSpinner;
    private ResourceType resourceType;
    public void Setup(ResourceType type, Sprite sprite, string name, int priority = 0){
        image.sprite = sprite;
        text.text = name;
        numberSpinner.Value = priority;

        numberSpinner.OnValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(int priority){
        UIResourcePrioritiesManager.current.UpdateResourcePriorities();
    }

    public int Priority => numberSpinner.Value;
    public ResourceType ResourceType => this.resourceType;
}
