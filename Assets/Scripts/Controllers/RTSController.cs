using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RTSController : MonoBehaviour {
    [SerializeField] GameObject selectionBoxObj;
    [SerializeField] GameObject arrowMoveObj;
    [SerializeField] Tilemap tilemapGround;
    List<Unit> unitSelectables = new List<Unit>();
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
            isSelectingBox = true;
            selectionBoxObj.SetActive(true);
            arrowMoveObj.SetActive(false);
            startSelectionBoxPosition = InputExtension.MouseWorldPoint();
        }
        if (isSelectingBox){
            if (Input.GetMouseButton(0)){
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
            if (Input.GetMouseButtonUp(0)){
                isSelectingBox = false;
                selectionBoxObj.SetActive(false);

                Collider2D[] collider2Ds = Physics2D.OverlapAreaAll(startSelectionBoxPosition, InputExtension.MouseWorldPoint());
                foreach (Unit prev in unitSelectables){
                    if (prev != null){
                        prev.OnDeselected();
                    }
                }
                unitSelectables.Clear();
                foreach (Collider2D collider in collider2Ds){
                    if (collider.TryGetComponent(out Unit unitSelectable)){
                        unitSelectable.OnSelected();
                        unitSelectables.Add(unitSelectable);
                    }
                }
            }
        }
    }
    void GiveOrdersControl(){
        if (!isSelectingBox && unitSelectables != null && unitSelectables.Count > 0){
            if (Input.GetMouseButtonDown(1)){
                Vector2 mouseWorldPosition = InputExtension.MouseWorldPoint();
                RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPosition, Vector3.forward);
                // TODO: kiem tra neu bam vao 1 phat thi mo control ui
                if (hits != null && hits.Length > 0) {
                    foreach (RaycastHit2D hit in hits) {
                        // neu bam vao building storage
                        if (hit.collider.TryGetComponent(out BuildingStorage storage)){
                            foreach (Unit selectable in unitSelectables){
                                if (selectable.TryGetComponent(out UnitTaskGathering gathering)){
                                    gathering.SetBuildingStorage(storage);
                                    gathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToStorage);
                                    gathering.StartDoTask();
                                }
                            }
                        }
                        // bam vao resource
                        if (hit.collider.TryGetComponent(out Resource resource)){
                            foreach (Unit selectable in unitSelectables){
                                if (selectable.TryGetComponent(out UnitTaskGathering gathering)){
                                    gathering.SetResource(resource);
                                    gathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToResource);
                                    gathering.StartDoTask();
                                }
                            }
                        }
                    }
                }
                else{
                    // just moving
                    foreach (Unit selectable in unitSelectables){
                        if (selectable.TryGetComponent(out IUnitTask task)){
                            task.EndDoTask();
                        }
                        if (selectable.TryGetComponent(out UnitMovement movement)){
                            movement.MoveToPosition(mouseWorldPosition);
                        }
                    }
                }
                arrowMoveObj.SetActive(true);
                arrowMoveObj.transform.DOKill();
                arrowMoveObj.transform.position = mouseWorldPosition + Vector2.up;
                arrowMoveObj.transform.DOMoveY(mouseWorldPosition.y, 0.5f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => DOVirtual.DelayedCall(3f, () => arrowMoveObj.SetActive(false)))
                    .Play();
            }
        }
    }
    void ZoomCameraControl(){
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0){
            float size = Camera.main.orthographicSize - scroll * 5;
            Camera.main.DOKill();
            Camera.main.DOOrthoSize(Mathf.Clamp(size, 10, 20), 0.2f).Play();
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
