using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class Building : MonoBehaviour {
    public Sprite icon;
    public string buildingName;
    [TextArea(3, 6)]
    public string description;
    public List<ResourceRequirement> resourceRequirements;
    public BoundsInt area;
    public bool IsPlaced { get;private set; }
    public abstract void OnBuildingPlaced();
    public bool CanBePlaced(){
        BoundsInt areaTemp = GridBuildingSystem.current.CalculateAreaFromWorldPosition(area, transform.position);

        if (GridBuildingSystem.current.CanTakeArea(areaTemp)){
            return true;
        }
        return false;
    }
    public void Place(){
        BoundsInt areaTemp = GridBuildingSystem.current.CalculateAreaFromWorldPosition(area, transform.position);

        IsPlaced = true;
        GridBuildingSystem.current.TakeArea(areaTemp);
        transform.DOKill();
        transform.DOMove(areaTemp.center, 0.1f).Play();


        OnBuildingPlaced();
    }
    public abstract void OnSelected();
    public abstract void OnDeselected();
}