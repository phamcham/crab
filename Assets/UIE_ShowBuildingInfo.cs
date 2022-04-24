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
    private void Awake() {
        requirementTrans.PCDestroyChildren();
    }
    public void Setup(string title, string description, List<ResourceRequirement> resourceRequirements){
        titleUpdateText?.Invoke(title);
        descriptionUpdateText?.Invoke(description);

        // BUG: can destroy het trc khi tao them
        foreach (ResourceRequirement req in resourceRequirements){
            UIE_ResourceRequirement ui = Instantiate(uieRequiredResourcePrefab.gameObject, requirementTrans)
                .GetComponent<UIE_ResourceRequirement>();
            ui.Setup(req);
        }
    }
}
