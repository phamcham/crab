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
    BoundsInt gridArea;
    public abstract void OnBuildingPlaced();
    public bool CanBePlaced() {
        gridArea = GridBuildingSystem.current.CalculateAreaFromWorldPosition(properties.area, transform.position);

        print(gridArea);

        if (GridBuildingSystem.current.CanTakeArea(gridArea)){
            return true;
        }
        return false;
    }
    public void Place(){
        IsPlaced = true;
        Debug.Log("Place: " + gridArea);
        GridBuildingSystem.current.TakeArea(gridArea);
        //transform.DOKill();
        //transform.DOMove((Vector2)gridArea.center, 0.1f).Play();
        transform.position = (Vector2)gridArea.center;
        NavMeshMap.current.UpdateNavMesh();

        properties.curHealthPoint = properties.maxHealthPoint;

        List<ResourceRequirement> requirements = properties.resourceRequirements;
        foreach (ResourceRequirement requirement in requirements) {
            ResourceType type = requirement.type;
            int amount = requirement.amount;

            ResourceManager.current.DeltaResource(type, -amount);
        }
        
        OnBuildingPlaced();
    }

    protected void OnDestroy() {
        if (!isApplicationQuit) {
            GridBuildingSystem.current.ClearArea(gridArea);
            NavMeshMap.current.UpdateNavMesh();
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
    public BoundsInt area;
    public List<ResourceRequirement> resourceRequirements;
}