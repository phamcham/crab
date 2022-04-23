using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResourcePriorities : MonoBehaviour {
    public static UI_ResourcePriorities current { get; private set; }

    [SerializeField] UIE_ResourcePrioritySetter setterPrefab;
    [SerializeField] Transform content;
    [SerializeField] Text infoText;
    Dictionary<ResourceType, UIE_ResourcePrioritySetter> setters = new Dictionary<ResourceType, UIE_ResourcePrioritySetter>();
    private void Awake() {
        current = this;
    }
    public void AddSetterUI(ResourceType resourceType, Sprite sprite, string name, int priority){
        if (setters.ContainsKey(resourceType)) {
            Debug.LogWarning(resourceType + " already added!");
            return;
        }
        UIE_ResourcePrioritySetter setter = Instantiate(setterPrefab, content).GetComponent<UIE_ResourcePrioritySetter>();
        setter.Setup(resourceType, sprite, name, priority);
        setters.Add(resourceType, setter);
    }

    public List<UIE_ResourcePrioritySetter> GetAllSetters(){
        return new List<UIE_ResourcePrioritySetter>(setters.Values);
    }
    public UIE_ResourcePrioritySetter GetSetter(ResourceType type){
        setters.TryGetValue(type, out UIE_ResourcePrioritySetter value);
        return value;
    }

    public void UpdateText(string s){
        infoText.text = s;
    }
}
