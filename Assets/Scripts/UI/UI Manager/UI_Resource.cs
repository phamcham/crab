using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;

public class UI_Resource : MonoBehaviour
{
    [SerializeField] Transform storingHolder;
    [SerializeField] UIE_NumberOfThings uie_NumberOfThingsPrefab;
    Dictionary<ResourceType, UIE_NumberOfThings> resources = new Dictionary<ResourceType, UIE_NumberOfThings>();
    private void Awake() {
        storingHolder.PCDestroyChildren();
    }
    public void UpdateResourceStoringUI(ResourceType type, int amount){
        if (!resources.TryGetValue(type, out UIE_NumberOfThings res)){
            res = Instantiate(uie_NumberOfThingsPrefab.gameObject, storingHolder)
                .GetComponent<UIE_NumberOfThings>();
            resources.Add(type, res);
        }
        Sprite sprite = ResourceManager.current.GetResourceSprite(type);
        res.Setup(sprite, amount);
    }
}
