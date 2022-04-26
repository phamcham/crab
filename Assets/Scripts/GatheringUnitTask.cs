using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GatheringUnitTask  {
    /*
    public const int MoveToResource = 2;
    public const int PickResource = 3;
    public const int BringResourceToHeadquarter = 4;
    public const int DropResourceToHeadquarter = 5;
    [SerializeField] SpriteRenderer holdingObject;
    ResourceTile TargetResource;
    float collectTime = 0;
    float curCollectTime;
    ResourceType holdingResourceType = ResourceType.None;
    bool isDoingTask = false;

    private void Start() {
        AddStep(DoNothing, DoStep_DoNothing);
        AddStep(MoveToResource, DoStep_MoveToResource);
        AddStep(PickResource, DoStep_PickResource);
        AddStep(BringResourceToHeadquarter, DoStep_BringResourceToHeadquarter);
        AddStep(DropResourceToHeadquarter, DoStep_DropResourceToHeadquarter);
    }

    private void DoStep_DoNothing(){
        if (!isDoingTask) return;
        unit.TargetPosition = transform.position;
    }
    private void DoStep_MoveToResource(){
        if (!isDoingTask) return;
        // go to resource
        //print("MoveToResource");
        if (TargetResource != null) {
            if (holdingResourceType == ResourceType.None && unit.PathResultType == PathResultType.IsCompleted) {
                if (curCollectTime >= collectTime) {
                    // chuyen su kien
                    curCollectTime = 0;
                    currentStep = PickResource;
                }
                else {
                    curCollectTime += Time.deltaTime;
                }
            }
            else {
                curCollectTime = 0;
            }
        }
        else {
            currentStep = DoNothing;
        }
    }

    private void DoStep_PickResource(){
        if (!isDoingTask) return;
        //print("PickResource");
        holdingObject.gameObject.SetActive(true);
        holdingObject.sprite = TargetResource.tile.sprite;
        holdingResourceType = TargetResource.type;
        HeadquarterBuilding headquarter = GameController.current.GetHeadquarterBuilding();
        unit.TargetPosition = (Vector2Int)headquarter.info.area.position;
            
        currentStep = BringResourceToHeadquarter;
    }

    private void DoStep_BringResourceToHeadquarter(){
        if (!isDoingTask) return;
        // back to base
        if (holdingResourceType != ResourceType.None){
            //print("BringResourceToHeadquarter");
            HeadquarterBuilding headquarter = GameController.current.GetHeadquarterBuilding();
            if (headquarter != null) {
                if (unit.PathResultType == PathResultType.IsCompleted) {
                    //Debug.Log("tim tiep thoi");
                    currentStep = DropResourceToHeadquarter;
                }
            }
        }
    }

    private void DoStep_DropResourceToHeadquarter(){
        if (!isDoingTask) return;
        //print("DropResourceToHeadquarter");
        if (TargetResource != null && holdingResourceType != ResourceType.None){
            //UI_ResourcesManager.current.GetResourceAmountUI(holdingResourceType).AddAmount(1);
            unit.TargetPosition = TargetResource.position;
            holdingResourceType = ResourceType.None;
            currentStep = MoveToResource;
        }
        else{
            currentStep = DoNothing;
        }
        holdingObject.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        if (Application.isPlaying){
            //ResourcePrioritiesManager.current.UnsubcribeGatherUnit(this);
        }
    }

    protected override void OnAwake() {}

    protected override void OnUpdate() {}

    public override void StartDoTask()
    {
        isDoingTask = true;
        currentStep = MoveToResource;
        TargetResource = ResourceManager.current.GetResourceTiles(ResourceType.Coconut)[0];
    }

    public override void EndDoTask()
    {
        isDoingTask = false;
    }
    */
}