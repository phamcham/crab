using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIE_CreateBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Button button;
    private Image image;
    private Building building;
    private Action onHover;
    private Action onExit;
    private void Awake() {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }
    public void Setup(Building building, Action onHover, Action onExit){
        this.building = building;
        this.onHover = onHover;
        this.onExit = onExit;

        image.sprite = this.building.GetSprite();

        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);

        UpdateUI();
    }
    public void UpdateUI() {
        List<ResourceRequirement> requirements = building.properties.resourceRequirements;
        bool canBuild = true;
        foreach (ResourceRequirement req in requirements) {
            ResourceType type = req.type;
            int amount = req.amount;
            int remain = ResourceManager.current.GetAmount(type);

            if (remain < amount) {
                canBuild = false;
                break;
            }
        }

        button.interactable = canBuild;
    }
    void OnClick(){
        GridBuildingSystem.current.InitializeWithBuilding(building);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (building != null){
            this.onHover?.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        this.onExit?.Invoke();
    }
}
