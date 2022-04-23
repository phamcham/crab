using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {
    public ResourceInfo info;
    public int amount;
    private SpriteRenderer spriteRenderer;
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start(){
        //info.area = GridBuildingSystem.current.CalculateAreaFromWorldPosition(info.area, transform.position);
    }
    public void Setup(ResourceInfo info, int amount){
        this.info = info;
        this.amount = amount;

        transform.position = (Vector2Int)info.area.position + new Vector2(0.5f, 0.5f);
        spriteRenderer.sprite = info.sprite;
    }
}

[System.Serializable]
public struct ResourceInfo {
    public ResourceType type;
    public BoundsInt area;
    public Sprite sprite;

    public ResourceInfo(ResourceType type, BoundsInt area, Sprite sprite)
    {
        this.type = type;
        this.area = area;
        this.sprite = sprite;
    }

    public ResourceInfo(ResourceInfo copy){
        this.type = copy.type;
        this.area = copy.area;
        this.sprite = copy.sprite;
    }
}