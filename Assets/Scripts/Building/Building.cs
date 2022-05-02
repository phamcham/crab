using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class Building : Entity {    
    public BuildingProperties properties;
    [SerializeField] SpriteRenderer spriteRenderer;
    public bool IsPlaced { get;private set; }
    bool isApplicationQuit = false;
    public abstract void OnBuildingPlaced();
    public bool CanBePlaced(){
        BoundsInt areaTemp = GridBuildingSystem.current.CalculateAreaFromWorldPosition(properties.area, transform.position);

        if (GridBuildingSystem.current.CanTakeArea(areaTemp)){
            return true;
        }
        return false;
    }
    public void Place(){
        BoundsInt areaTemp = GridBuildingSystem.current.CalculateAreaFromWorldPosition(properties.area, transform.position);

        IsPlaced = true;
        GridBuildingSystem.current.TakeArea(areaTemp);
        transform.DOKill();
        transform.DOMove(areaTemp.center, 0.1f).Play();

        properties.curHealthPoint = properties.maxHealthPoint;
        
        OnBuildingPlaced();
    }
    protected void OnDestroy() {
        if (!isApplicationQuit) {
            BoundsInt areaTemp = GridBuildingSystem.current.CalculateAreaFromWorldPosition(properties.area, transform.position);

            GridBuildingSystem.current.ClearArea(areaTemp);
            OnDestroyBuilding();
        }
    }
    protected abstract void OnDestroyBuilding();
    public Sprite GetSprite() {
        return spriteRenderer.sprite;
    }
    private void OnApplicationQuit() {
        isApplicationQuit = true;
    }
}

[System.Serializable]
public struct BuildingProperties {
    public string buildingName;
    [TextArea(3, 5)]
    public string description;
    public int maxHealthPoint;
    [HideInInspector]
    public int curHealthPoint;
    public int defendPoint;
    public BoundsInt area;
    public List<ResourceRequirement> resourceRequirements;
}