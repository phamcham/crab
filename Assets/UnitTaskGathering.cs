using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PhamCham.Extension;
using UnityEngine;

public class UnitTaskGathering : MonoBehaviour, IUnitTask {
    public enum TaskGatheringState {
        MoveToResource,
        PickupResource,
        MoveToStorage,
        DropoffResource,
        DoNothing
    }
    [SerializeField] SpriteRenderer holdingObjectSpriteRenderer;
    private ResourceType currentResourceIsHolding = ResourceType.None;
    private ResourceType currentTargetType;
    private Resource currentTargetResource;
    private UnitMovement movement;
    private Unit unit;
    private BuildingStorage storage;
    private TaskGatheringState currentState = TaskGatheringState.DoNothing;
    private bool initStep = false;
    protected Dictionary<TaskGatheringState, Action> steps = new Dictionary<TaskGatheringState, Action>();
    public void SetResourceType(ResourceType type){
        currentTargetType = type;
    }
    public void SetResource(Resource resource){
        currentTargetResource = resource ?? GetRandomResource();
        currentTargetType = currentTargetResource.GetResourceType();
    }
    public void SetBuildingStorage(BuildingStorage storage){
        this.storage = storage ?? GetRandomBuildingStorage();
    }
    public void SetState(TaskGatheringState startState){
        currentState = startState;
    }
    private BuildingStorage GetRandomBuildingStorage(){
        List<BuildingStorage> storages = FindObjectsOfType<BuildingStorage>()?.ToList();
        return storages?.PCItemRandom();
    }
    private Resource GetRandomResource(){
        List<ResourceType> list = (Enum.GetValues(typeof(ResourceType)) as IEnumerable<ResourceType>).Cast<ResourceType>().ToList();
        List<Resource> tiles = ResourceManager.current.GetResources(list.PCItemRandom());
        return tiles?.PCItemRandom();
    }
    public void EndDoTask() {
        enabled = false;
    }

    public void StartDoTask() {
        enabled = true;
        initStep = true;
    }

    private void Awake() {
        unit = GetComponent<Unit>();
        movement = GetComponent<UnitMovement>();
    }
    private void Start() {
        steps.Add(TaskGatheringState.MoveToResource, MoveToResourceAction);
        steps.Add(TaskGatheringState.PickupResource, PickupResourceAction);
        steps.Add(TaskGatheringState.MoveToStorage, MoveToStorageAction);
        steps.Add(TaskGatheringState.DropoffResource, DropoffResourceAction);

        EndDoTask();
    }
    private void Update() {
        if (steps.TryGetValue(currentState, out Action action)){
            action?.Invoke();
            //print(currentStep);
        }
    }
    private void MoveToResourceAction() {
        if (initStep) {
            initStep = false;
            if (!currentTargetResource){
                currentTargetResource = GetRandomResource();
            }
            if (currentTargetResource){
                movement.MoveToPosition(currentTargetResource.transform.position);
            }
        }
        if (currentTargetResource != null) {
            //print(movement.PathResultType);
            if (movement.PathResultType == PathResultType.IsCompleted) {
                currentState = TaskGatheringState.PickupResource;
                initStep = true;
            }
        }
        else {
            // doi resource khac
            initStep = true;
        }
    }
    private void PickupResourceAction() {
        if (initStep){
            initStep = false;
        }
        if (currentTargetResource != null) {
            //currentTargetResource.amount -= 1;
            currentResourceIsHolding = currentTargetResource.GetResourceType();
            holdingObjectSpriteRenderer.gameObject.SetActive(true);
            holdingObjectSpriteRenderer.sprite = currentTargetResource.GetSprite();
            initStep = true;
            currentState = TaskGatheringState.MoveToStorage;
        }
    }
    private void MoveToStorageAction() {
        if (initStep){
            initStep = false;
            // TODO: find shortest storage
            if (!storage){
                storage = GetRandomBuildingStorage();
            }
            if (storage){
                movement.MoveToPosition(storage.transform.position);
            }
        }
        if (movement.PathResultType == PathResultType.IsCompleted){
            initStep = true;
            currentState = TaskGatheringState.DropoffResource;
        }
    }
    private void DropoffResourceAction() {
        if (initStep){
            initStep = false;
        }
        holdingObjectSpriteRenderer.sprite = null;
        holdingObjectSpriteRenderer.gameObject.SetActive(false);
        initStep = true;
        currentState = TaskGatheringState.MoveToResource;
        if (currentResourceIsHolding != ResourceType.None){
            ResourceManager.current.CollectResource(currentResourceIsHolding, 1);
        }
    }

}
