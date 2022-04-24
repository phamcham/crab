using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResourcesManager : MonoBehaviour {
    //public static UI_ResourcesManager current { get; private set; }

    [SerializeField] UIE_ResourcePrioritySetter setterPrefab;
    [SerializeField] Transform setterContentHolder;
    [SerializeField] UIE_ResourceAmount amountPrefab;
    [SerializeField] Transform amountContentHolder;
    [SerializeField] Text setterInfoText;
    Dictionary<ResourceType, UIE_ResourcePrioritySetter> setters = new Dictionary<ResourceType, UIE_ResourcePrioritySetter>();
    Dictionary<ResourceType, UIE_ResourceAmount> amounts = new Dictionary<ResourceType, UIE_ResourceAmount>();
    private void Awake() {
        //current = this;
        setterContentHolder.PCDestroyChildren();
        amountContentHolder.PCDestroyChildren();
    }
    public void AddResourceUI(ResourceType resourceType, Sprite sprite, string name){
        if (setters.ContainsKey(resourceType)) {
            Debug.LogWarning(resourceType + " already added!");
            return;
        }
        UIE_ResourcePrioritySetter setter = Instantiate(setterPrefab, setterContentHolder).GetComponent<UIE_ResourcePrioritySetter>();
        setter.Setup(resourceType, sprite, name);
        setters.Add(resourceType, setter);

        if (amounts.ContainsKey(resourceType)) {
            Debug.LogWarning(resourceType + " already added!");
            return;
        }
        UIE_ResourceAmount amount = Instantiate(amountPrefab, amountContentHolder).GetComponent<UIE_ResourceAmount>();
        amount.Setup(resourceType, sprite, name);
        amounts.Add(resourceType, amount);
    }

    public List<UIE_ResourcePrioritySetter> GetAllSetters(){
        return new List<UIE_ResourcePrioritySetter>(setters.Values);
    }
    public UIE_ResourcePrioritySetter GetResourcePrioritySetterUI(ResourceType type){
        setters.TryGetValue(type, out UIE_ResourcePrioritySetter value);
        return value;
    }
    public UIE_ResourceAmount GetResourceAmountUI(ResourceType type){
        amounts.TryGetValue(type, out UIE_ResourceAmount value);
        return value;
    }

    public void UpdateText(string s){
        setterInfoText.text = s;
    }
}
