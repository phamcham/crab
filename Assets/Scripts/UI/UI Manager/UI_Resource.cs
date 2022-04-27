using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;

public class UI_Resource : MonoBehaviour
{
    [SerializeField] Transform storingHolder;
    [SerializeField] UIE_ResourceStoring uie_ResourceStoringPrefab;

    Dictionary<ResourceType, UIE_ResourceStoring> resources = new Dictionary<ResourceType, UIE_ResourceStoring>();
    private void Awake() {
        storingHolder.PCDestroyChildren();
    }
    public void UpdateResourceStoringUI(ResourceType type, int amount){
        if (!resources.TryGetValue(type, out UIE_ResourceStoring res)){
            res = Instantiate(uie_ResourceStoringPrefab.gameObject, storingHolder)
                .GetComponent<UIE_ResourceStoring>();
            resources.Add(type, res);
        }
        res.Setup(type, amount);
    }
}
