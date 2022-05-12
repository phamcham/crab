using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PhamCham.Extension;
using UnityEngine;

public class UnitTaskGathering : MonoBehaviour, IUnitTask {
    // ====== task properties =========
    public enum TaskGatheringState {
        MoveToResource,
        PickupResource,
        MoveToStorage,
        DropoffResource,
        DoNothing
    }
    public bool IsTaskRunning { get; internal set; }
    // =========== ref ==============
    [SerializeField] SpriteRenderer holdingObjectSpriteRenderer;
    // =========== public ============
    public Unit BaseUnit { get; private set; }
    // ======== private ===========
    private int currentResourceIsHoldingCount = 0;
    private bool initStep = false;
    private TaskGatheringState currentState = TaskGatheringState.DoNothing;
    private ResourceType currentResourceIsHolding = ResourceType.None;
    private ResourceType currentTargetType;
    private Resource currentTargetResource;
    private UnitNavMovement movement;
    private BuildingStorage storage;
    private Animation anim;
    private Dictionary<TaskGatheringState, Action> steps = new Dictionary<TaskGatheringState, Action>();
    Tween continueGatheringIfIdleTween;
    // ======== methods ============
    public void SetResourceType(ResourceType type){
        currentTargetType = type;
    }
    public void SetResource(Resource resource){
        currentTargetResource = resource ?? GetRandomResource();
        currentTargetType = currentTargetResource.properties.type;
    }
    public void SetBuildingStorage(BuildingStorage storage){
        this.storage = storage ?? GetRandomBuildingStorage();
    }
    public void SetState(TaskGatheringState startState){
        currentState = startState;
        initStep = true;
    }
    private BuildingStorage GetRandomBuildingStorage(){
        List<BuildingStorage> storages = FindObjectsOfType<BuildingStorage>()?.ToList();
        storages = storages.Where(s => s.CanStoraged).ToList();
        return storages?.PCItemRandom();
    }
    private Resource GetRandomResource(){
        List<ResourceType> curTypes = ResourceManager.current.GetResourceTypes();
        List<Resource> tiles = ResourceManager.current.GetResources(curTypes.PCItemRandom());
        return tiles?.PCItemRandom();
    }
    public void EndDoTask() {
        if (!IsTaskRunning) return;
        IsTaskRunning = false;
    }

    public void StartDoTask() {
        if (IsTaskRunning) return;
        // stop all tasks before do this task
        //enabled = true;
        IsTaskRunning = true;
        initStep = true;
    }

    private void Awake() {
        BaseUnit = GetComponent<Unit>();
        movement = GetComponent<UnitNavMovement>();
        anim = GetComponent<Animation>();
    }
    private void Start() {
        steps.Add(TaskGatheringState.MoveToResource, MoveToResourceAction);
        steps.Add(TaskGatheringState.PickupResource, PickupResourceAction);
        steps.Add(TaskGatheringState.MoveToStorage, MoveToStorageAction);
        steps.Add(TaskGatheringState.DropoffResource, DropoffResourceAction);

        EndDoTask();
    }
    private void Update() {
        if (IsTaskRunning) {
            if (steps.TryGetValue(currentState, out Action action)){
                action?.Invoke();
                //print(currentStep);
            }
        }
    }
    private void MoveToResourceAction() {
        if (initStep) {
            initStep = false;
            if (!currentTargetResource){
                currentTargetResource = GetRandomResource();
            }
            if (currentTargetResource){
                movement.Move(currentTargetResource.transform.position);
            }
        }
        if (currentTargetResource != null) {
            //print(movement.PathResultType);
            float distance = Vector2.Distance(transform.position, currentTargetResource.transform.position);
            // if (distance < 1f && movement.PathResultType == PathResultType.IsCompleted) {
            //     SetState(TaskGatheringState.PickupResource);
            // }
            if (movement.IsReachedDestination()) {
                SetState(TaskGatheringState.PickupResource);
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
            anim.Stop();
            anim.Play("crab gathering");
        }

        if (currentTargetResource != null) {
            if (currentTargetResource.TryGrathering()) {
                currentResourceIsHoldingCount = 1;
                currentTargetResource.Pickup(currentResourceIsHoldingCount);

                currentResourceIsHolding = currentTargetResource.properties.type;
                holdingObjectSpriteRenderer.gameObject.SetActive(true);
                holdingObjectSpriteRenderer.sprite = currentTargetResource.GetSprite();

                SetState(TaskGatheringState.MoveToStorage);
                anim.Stop();
            }
        }
        else {
            SetState(TaskGatheringState.MoveToResource);
            anim.Stop();
        }
    }
    private void MoveToStorageAction() {
        if (initStep){
            // TODO: find shortest storage
            if (!storage){
                storage = GetRandomBuildingStorage();
            }
            if (storage){
                initStep = false;
                movement.Move(storage.transform.position);
            }
        }
        if (storage) {
            float distance = Vector2.Distance(storage.transform.position, transform.position);
            // if (distance <= 1f && movement.PathResultType == PathResultType.IsCompleted){
            //     SetState(TaskGatheringState.DropoffResource);
            // }
            if (movement.IsReachedDestination()) {
                SetState(TaskGatheringState.DropoffResource);
            }
        }
    }
    private void DropoffResourceAction() {
        if (initStep){
            initStep = false;
        }
        holdingObjectSpriteRenderer.sprite = null;
        holdingObjectSpriteRenderer.gameObject.SetActive(false);
        SetState(TaskGatheringState.MoveToResource);
        if (currentResourceIsHolding != ResourceType.None){
            ResourceManager.current.DeltaResource(currentResourceIsHolding, 1);
        }
    }

    private void OnDestroy() {
        continueGatheringIfIdleTween.Kill();
    }
}
