using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Entity, ISelectable, ISaveObject<ResourceSaveData> {
    public override Team Team => Team.DefaultPlayer;
    public ResourceProperties properties;
    [SerializeField] GameObject selectorObj;
    [SerializeField] HealthBar gratheringProcessbar;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer spriteRenderer {
        get {
            if (_spriteRenderer == null){
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return _spriteRenderer;
        }
    }

    UIE_ResourceControl uiControl;
    private void Start() {
        uiControl = SelectionOneUIControlManager.current.GetUIControl<UIE_ResourceControl>(this);
        uiControl.SetResource(this);
        uiControl.Hide();

        properties.currentGatheringTime = properties.gatheringTime;
        gratheringProcessbar.SetSize(0);
        gratheringProcessbar.Hide();
        selectorObj.SetActive(false);
    }
    public Sprite GetSprite(){
        return spriteRenderer.sprite;
    }

    public void SetAmount(int amount) {
        properties.amount = amount;

        if (amount == 0) {
            Destroy(gameObject);
        }
    }

    public void Pickup(int amount) {
        properties.amount -= amount;
        
        properties.currentGatheringTime = properties.gatheringTime;
        gratheringProcessbar.SetSize(0);
        gratheringProcessbar.Hide();
        
        if (properties.amount <= 0) {
            Destroy(gameObject);
        }
    }

    // call in update step of gatherer,
    public bool TryGrathering() {
        if (properties.currentGatheringTime <= 0) return true;
        properties.currentGatheringTime = Mathf.Max(0, properties.currentGatheringTime - Time.deltaTime);
        gratheringProcessbar.SetSize(1f - properties.currentGatheringTime / properties.gatheringTime);
        return false;
    }

    public void OnSelected() {
        selectorObj.SetActive(true);
    }

    public void OnDeselected() {
        selectorObj.SetActive(false);
    }

    public void OnGiveOrder(Vector2 position) {
        // do nothing
    }

    public void OnShowControlUI(bool isShow) {
        if (isShow) uiControl.Show();
        else uiControl.Hide();
    }

    private void OnDestroy() {
        SelectionOneUIControlManager.current.RemoveUIControl(this);
    }

    public ResourceSaveData GetSaveObjectData() {
        return new ResourceSaveData() {
            type = properties.type,
            gatheringTime = properties.gatheringTime,
            currentGatheringTime = properties.currentGatheringTime,
            amount = properties.amount,
            position = new SaveSystemExtension.Vector2(transform.position)
        };
    }
}

[System.Serializable]
public struct ResourceProperties {
    public string nameResource;
    public ResourceType type;
    public float gatheringTime;
    [HideInInspector] public float currentGatheringTime; // if currentGatheringTime = 0 then gratherer can pick it up immediately 
    [HideInInspector] public int amount;
}

[System.Serializable]
public struct ResourceSaveData {
    public ResourceType type;
    public float gatheringTime;
    public float currentGatheringTime;
    public int amount;
    public SaveSystemExtension.Vector2 position;
}