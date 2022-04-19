using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BoundsInt area = new BoundsInt(Vector3Int.zero, Vector3Int.one);
    public bool IsPlaced { get;private set; }
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
    }
}
