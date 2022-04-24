using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIE_CreateBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    Button button;
    Building building;
    Action onHover;
    Action onExit;
    private void Awake() {
        button = GetComponent<Button>();
    }
    public void Setup(Building building, Action onHover, Action onExit){
        this.building = building;
        this.onHover = onHover;
        this.onExit = onExit;
        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }
    void OnClick(){
        GridBuildingSystem.current.InitializeWithBuilding(building.gameObject);
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
