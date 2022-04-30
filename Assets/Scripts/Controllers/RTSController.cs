using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RTSController : MonoBehaviour {
    [SerializeField] GameObject selectionBoxObj;
    [SerializeField] GameObject arrowMoveObj;
    [SerializeField] Tilemap tilemapGround;
    List<Unit> selectedUnits = new List<Unit>();
    private Building onlyOneSelectedBuilding;
    //private bool isMultiSelect;
    Vector3 startSelectionBoxPosition;
    Vector3 startMoveCameraPosition;
    bool isSelectingBox = false;
    bool isMoveCamera = false;
    private void Awake() {
        selectionBoxObj.SetActive(false);
        arrowMoveObj.SetActive(false);
    }
    private void Update() {
        SelectionBoxControl();
        GiveOrdersControl();
    }

    private void LateUpdate() {
        MoveCameraControl();
        ZoomCameraControl();
    }

    void SelectionBoxControl(){
        if (Input.GetMouseButtonDown(0) && !InputExtension.IsMouseOverUI()){
            StartDraggingSelectionBox();
        }
        if (isSelectingBox){
            if (Input.GetMouseButton(0)){
                DraggingSelectionBox();
            }
            if (Input.GetMouseButtonUp(0)){
                Collider2D[] collider2Ds = Physics2D.OverlapAreaAll(startSelectionBoxPosition, InputExtension.MouseWorldPoint());
                DeselectPrevDraggingSelectionBox();
                if (collider2Ds != null && collider2Ds.Length > 0) {
                    // TODO: kiem tra neu bam vao 1 phat thi mo control ui, neu k mo multiselect
                    if (collider2Ds.Length == 1) {
                        if (collider2Ds[0].TryGetComponent(out Building building)){
                            SelectOneThing(building);
                        }
                        else if (collider2Ds[0].TryGetComponent(out Unit unit)) {
                            SelectOneThing(unit);
                        }
                    }
                    else if (collider2Ds.Length > 1) {
                        List<Unit> units = new List<Unit>();
                        foreach (Collider2D collider in collider2Ds){
                            if (collider.TryGetComponent(out Unit unit)){
                                units.Add(unit);
                            }
                        }
                        SelectMoreThings(units);
                    }
                }
            }
        }
    }
    private void StartDraggingSelectionBox() {
        isSelectingBox = true;
        selectionBoxObj.SetActive(true);
        arrowMoveObj.SetActive(false);
        startSelectionBoxPosition = InputExtension.MouseWorldPoint();
    }
    private void DraggingSelectionBox() {
        Vector3 currentPosition = InputExtension.MouseWorldPoint();
        Vector3 lowerLeft = new Vector3(
            Mathf.Min(startSelectionBoxPosition.x, currentPosition.x),
            Mathf.Min(startSelectionBoxPosition.y, currentPosition.y)
        );
        Vector3 upperRight = new Vector3(
            Mathf.Max(startSelectionBoxPosition.x, currentPosition.x),
            Mathf.Max(startSelectionBoxPosition.y, currentPosition.y)
        );
        selectionBoxObj.transform.position = lowerLeft;
        selectionBoxObj.transform.localScale = upperRight - lowerLeft;
    }
    private void DeselectPrevDraggingSelectionBox() {
        isSelectingBox = false;
        selectionBoxObj.SetActive(false);

        foreach (Unit prev in selectedUnits){
            if (prev != null){
                prev.OnDeselected();
                prev.ShowControlUI(false);
            }
        }
        selectedUnits.Clear();
        if (onlyOneSelectedBuilding) {
            print("deselect building");
            onlyOneSelectedBuilding.ShowControlUI(false);
            onlyOneSelectedBuilding = null;
        }
    }
    private void SelectOneThing(Building building) {
        // on 1 building selected
        building.OnSelected();
        building.ShowControlUI(true);
        onlyOneSelectedBuilding = building;
    }
    private void SelectOneThing(Unit unit) {
        unit.OnSelected();
        unit.ShowControlUI(true);
        selectedUnits.Add(unit);
    }
    private void SelectMoreThings(List<Unit> units) {
        if (units.Count == 1) {
            SelectOneThing(units[0]);
        }
        else {
            foreach (Unit unit in units) {
                unit.OnSelected();
                selectedUnits.Add(unit);
                // no show control ui here
            }
        }
    }
    void GiveOrdersControl(){
        if (!isSelectingBox && Input.GetMouseButtonDown(1)){
            if (onlyOneSelectedBuilding) {
                // vua bam vao building sau do bam chuot ra cho khac => cha lam j ca
                onlyOneSelectedBuilding.OnDeselected();
                onlyOneSelectedBuilding.ShowControlUI(false);
                onlyOneSelectedBuilding = null;
            }
            else if (selectedUnits != null && selectedUnits.Count > 0) {
                // dang select cac unit
                Vector2 mouseWorldPosition = InputExtension.MouseWorldPoint();
                RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPosition, Vector3.forward);
                if (hits != null && hits.Length > 0) {
                    foreach (RaycastHit2D hit in hits) {
                        // neu bam vao building storage
                        if (hit.collider.TryGetComponent(out BuildingStorage storage)){
                            GiveOrderByBuildingStorage(storage);
                            break;
                        }
                        // bam vao resource
                        if (hit.collider.TryGetComponent(out Resource resource)){
                            GiveOrderByResource(resource);
                            break;
                        }
                    }
                }
                else{
                    // just moving
                    GiveOrderByMoving(mouseWorldPosition);
                }
                ShowArrow(mouseWorldPosition);
            }
        }
        
    }
    private void GiveOrderByBuildingStorage(BuildingStorage storage) {
        foreach (Unit selectable in selectedUnits){
            if (selectable.TryGetComponent(out UnitTaskGathering gathering)){
                gathering.SetBuildingStorage(storage);
                gathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToStorage);
                gathering.StartDoTask();
            }
        }
    }
    private void GiveOrderByResource(Resource resource) {
        foreach (Unit selectable in selectedUnits){
            if (selectable.TryGetComponent(out UnitTaskGathering gathering)){
                gathering.SetResource(resource);
                gathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToResource);
                gathering.StartDoTask();
            }
        }
    }
    private void GiveOrderByMoving(Vector2 position) {
        foreach (Unit selectable in selectedUnits){
            if (selectable.TryGetComponent(out IUnitTask task)){
                task.EndDoTask();
            }
            if (selectable.TryGetComponent(out UnitMovement movement)){
                movement.MoveToPosition(position);
            }
        }
    }
    private void ShowArrow(Vector2 position) {
        arrowMoveObj.SetActive(true);
        arrowMoveObj.transform.DOKill();
        arrowMoveObj.transform.position = position + Vector2.up;
        arrowMoveObj.transform.DOMoveY(position.y, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => DOVirtual.DelayedCall(3f, () => arrowMoveObj.SetActive(false)))
            .Play();
    }
    void ZoomCameraControl(){
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0){
            float size = Camera.main.orthographicSize - scroll * 3;
            Camera.main.DOKill();
            Camera.main.DOOrthoSize(Mathf.Clamp(size, 5, 20), 0.2f).Play();
        }
    }
    void MoveCameraControl(){
        if (Input.GetMouseButtonDown(2) && !InputExtension.IsMouseOverUI()){
            startMoveCameraPosition = InputExtension.MouseWorldPoint();
            isMoveCamera = true;
        }
        if (isMoveCamera){
            if (Input.GetMouseButton(2)){
                Vector3 currentMousePosition = InputExtension.MouseWorldPoint();
                Vector3 delta = startMoveCameraPosition - currentMousePosition;
                Camera.main.transform.position += delta;
            }
            if (Input.GetMouseButtonUp(1)){
                isMoveCamera = false;
            }
        }
    }
}
