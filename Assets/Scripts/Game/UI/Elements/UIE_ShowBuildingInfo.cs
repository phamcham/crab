using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.Events;

public class UIE_ShowBuildingInfo : MonoBehaviour
{
    [SerializeField] UnityEvent<string> titleUpdateText;
    [SerializeField] UnityEvent<string> descriptionUpdateText;
    [SerializeField] Transform requirementTrans;
    [SerializeField] UIE_ResourceRequirement uieRequiredResourcePrefab;
    Dictionary<ResourceType, UIE_ResourceRequirement> requirements = new Dictionary<ResourceType, UIE_ResourceRequirement>();
    private void Awake() {
        requirementTrans.PCDestroyChildren();
    }

    public void Setup(string title, string description, List<ResourceRequirement> resourceRequirements){
        titleUpdateText?.Invoke(title);
        descriptionUpdateText?.Invoke(description);

        foreach (UIE_ResourceRequirement uie in requirements.Values){
            //Debug.Log("aaa : " + uie.name);
            uie.gameObject.SetActive(false);
        }
        foreach (ResourceRequirement req in resourceRequirements){
            if (!requirements.TryGetValue(req.type, out UIE_ResourceRequirement ui)){
                ui = Instantiate(uieRequiredResourcePrefab.gameObject, requirementTrans).GetComponent<UIE_ResourceRequirement>();
                requirements.Add(req.type, ui);
                //print(req.type + " " + req.amount);
            }
            ui.gameObject.SetActive(true);
            ui.Setup(req);
        }
    }
}
