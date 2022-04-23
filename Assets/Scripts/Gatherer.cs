using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gatherer : MonoBehaviour {
    [SerializeField] SpriteRenderer holdingObject;
    private Unit unit;
    Resource TargetResource;
    float collectTime = 0;
    float curCollectTime;
    //LinkedList<Action> gathererActions = new LinkedList<Action>();
    Dictionary<WorkType, Action> works = new Dictionary<WorkType, Action>();
    //LinkedListNode<Action> currentAction;
    WorkType currentWork = WorkType.DoNothing;
    ResourceType holdingResourceType = ResourceType.None;
    private void Awake() {
        unit = GetComponent<Unit>();
    }

    private void Start() {
        ResourcePrioritiesManager.current.SubscribeGathererUnit(this);

        works.Add(WorkType.DoNothing, DoWork_DoNothing);
        works.Add(WorkType.MoveToResource, DoWork_MoveToResource);
        works.Add(WorkType.PickResource, DoWork_PickResource);
        works.Add(WorkType.BringResourceToHeadquarter, DoWork_BringResourceToHeadquarter);
        works.Add(WorkType.DropResourceToHeadquarter, DoWork_DropResourceToHeadquarter);

        currentWork = WorkType.DoNothing;
    }

    private void DoWork_DoNothing(){
        unit.TargetPosition = transform.position;
    }
    private void DoWork_MoveToResource(){
        // go to resource
        //print("MoveToResource");
        if (TargetResource != null) {
            if (holdingResourceType == ResourceType.None && unit.PathResultType == PathResultType.IsCompleted) {
                if (curCollectTime >= collectTime) {
                    // chuyen su kien
                    curCollectTime = 0;
                    currentWork = WorkType.PickResource;
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
            currentWork = WorkType.DoNothing;
        }
    }

    private void DoWork_PickResource(){
        //print("PickResource");
        holdingObject.gameObject.SetActive(true);
        holdingObject.sprite = TargetResource.info.sprite;
        holdingResourceType = TargetResource.info.type;
        HeadquarterBuilding headquarter = GameController.current.GetHeadquarterBuilding();
        unit.TargetPosition = (Vector2Int)headquarter.area.position;
            
        currentWork = WorkType.BringResourceToHeadquarter;
    }

    private void DoWork_BringResourceToHeadquarter(){
        // back to base
        if (holdingResourceType != ResourceType.None){
            //print("BringResourceToHeadquarter");
            HeadquarterBuilding headquarter = GameController.current.GetHeadquarterBuilding();
            if (headquarter != null) {
                if (unit.PathResultType == PathResultType.IsCompleted) {
                    //Debug.Log("tim tiep thoi");
                    currentWork = WorkType.DropResourceToHeadquarter;
                }
            }
        }
    }

    private void DoWork_DropResourceToHeadquarter(){
        //print("DropResourceToHeadquarter");
        holdingObject.gameObject.SetActive(false);
        holdingResourceType = ResourceType.None;
        if (TargetResource != null){
            unit.TargetPosition = (Vector2Int)TargetResource.info.area.position;
            currentWork = WorkType.MoveToResource;
        }
        else{
            currentWork = WorkType.DoNothing;
        }
    }

    private void OnDestroy() {
        if (Application.isPlaying){
            ResourcePrioritiesManager.current.UnsubcribeGatherUnit(this);
        }
    }

    private void Update() {
        works[currentWork]?.Invoke();
    }

    public void OnChangeTargetResourceType(ResourceType resourceType){
        if (resourceType == ResourceType.None){
            if (holdingResourceType == ResourceType.None){
                // do nothing
                currentWork = WorkType.DoNothing;
            }
            else{
                TargetResource = null;
            }
        }
        else {
            List<Resource> resources = GridResourcesSystem.current.GetResources(resourceType);
            if (resources != null){
                int index = UnityEngine.Random.Range(0, resources.Count);
                //print(index);
                //print(string.Join(", ", resources.Select(x => x.info.area.position)));
                TargetResource = resources[index];
                //unit.TargetPosition = TargetResource.transform.position;
                if (holdingResourceType == ResourceType.None){
                    currentWork = WorkType.MoveToResource;
                    unit.TargetPosition = (Vector2Int)TargetResource.info.area.position;
                }
                else{
                    currentWork = WorkType.BringResourceToHeadquarter;
                    HeadquarterBuilding headquarter = GameController.current.GetHeadquarterBuilding();
                    unit.TargetPosition = (Vector2Int)headquarter.area.position;
                }
            }
            else{
                Debug.LogError("null");
            }
        }
    }
    void FindTargetResourcePosition(){
        Vector2Int position = (Vector2Int)TargetResource.info.area.position;
        unit.TargetPosition = position;
    }

    public enum WorkType {
        DoNothing,
        MoveToResource,
        PickResource,
        BringResourceToHeadquarter,
        DropResourceToHeadquarter,
    }
}
/*
public class CircularActionsOnUpdate {
    CircularAction first, last;
    CircularAction cur;

    // call in update
    public void Update(){
        cur.action?.Invoke();
    }

    public CircularActionsOnUpdate AddAction(Action action){
        CircularAction add = new CircularAction(action, first);
        if (last == null){
            last = add;
        }

        last.next = add;
        last = add;

        if (first == null){
            first = cur = last;
        }

        return this;
    }

    public void MoveNext(){
        cur = cur.next;
    }

    public class CircularAction {
        public Action action;
        public CircularAction next;
        public CircularAction(Action action, CircularAction next){
            this.action = action;
            this.next = next;
        }
    }
}*/