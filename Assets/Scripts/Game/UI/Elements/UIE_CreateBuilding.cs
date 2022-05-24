using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIE_CreateBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] Image image;
    [SerializeField] TMPro.TextMeshProUGUI shortKeyText;
    private Button button;
    private Building building;
    private Action onHover;
    private Action onExit;
    private KeyCode shortkey;
    private void Awake() {
        button = GetComponent<Button>();
    }
    public void Setup(Building building, Action onHover, Action onExit, KeyCode shortkey){
        this.building = building;
        this.onHover = onHover;
        this.onExit = onExit;
        this.shortkey = shortkey;

        shortKeyText.text = shortkey.ToString();
        image.sprite = this.building.GetSprite();
        image.GetComponent<AspectRatioFitter>().aspectRatio = image.sprite.rect.width / image.sprite.rect.height;

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

    private void Update() {
        if (Input.GetKeyDown(shortkey)) {
            OnClick();
        }
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
