using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RTSController : MonoBehaviour {
    [SerializeField] GameObject selectionBoxObj;
    [SerializeField] GameObject arrowMoveObj;
    [SerializeField] Tilemap tilemapGround;
    public bool controlMoveCameraByEdgeScreen = false;
    public float moveCamSpeed;
    List<ISelectable> selectables = new List<ISelectable>();
    //private bool isMultiSelect;
    Vector3 startSelectionBoxPosition;
    Vector3 startMoveCameraPosition;
    bool isSelectingBox = false;
    private void Awake() {
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Start() {
        selectionBoxObj.SetActive(false);
        arrowMoveObj.SetActive(false);    
    }
    private void Update() {
        if (!GameController.current.IsGameplayPaused) {
            SelectionBoxControl();
            GiveOrdersControl();
            
            MoveCameraControl();
            ZoomCameraControl();
        }
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
                DeselectPrevDraggingSelectionBox();
                SelecteDraggingSelectionBox(startSelectionBoxPosition, InputExtension.MouseWorldPoint());
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

        foreach (ISelectable prev in selectables){
            if (prev != null && !prev.Equals(null)) {
                prev.OnDeselected();
                prev.OnShowControlUI(false);
            }
        }
        selectables.Clear();
    }
    private void SelecteDraggingSelectionBox(Vector2 pointA, Vector2 pointB) {
        Collider2D[] collider2Ds = Physics2D.OverlapAreaAll(pointA, pointB);
                
        if (collider2Ds != null && collider2Ds.Length > 0) {
            // TODO: kiem tra neu bam vao 1 phat thi mo control ui, neu k mo multiselect
            // Check all units selected
            List<ISelectable> playerUnits = new List<ISelectable>();
            List<ISelectable> buildings = new List<ISelectable>();
            foreach (Collider2D collider in collider2Ds) {
                if (collider.TryGetComponent(out ISelectable selectable)) {
                    if (collider.TryGetComponent(out PlayerUnit unit)) {
                        playerUnits.Add(selectable);
                    }
                    else if (collider.TryGetComponent(out Building building)) {
                        buildings.Add(selectable);
                    }
                }
            }

            if (playerUnits.Count > 0) {
                if (playerUnits.Count == 1) {
                    playerUnits[0].OnShowControlUI(true);
                }
                foreach (ISelectable unit in playerUnits) {
                    unit.OnSelected();
                    selectables.Add(unit);
                }
            }
            else if (buildings.Count == 1) {
                ISelectable building = buildings[0];
                building.OnSelected();
                building.OnShowControlUI(true);
                selectables.Add(building);
            }
        }
    }
    void GiveOrdersControl(){
        if (!isSelectingBox && Input.GetMouseButtonDown(1)){
            //print("order: " + selectables.Count);
            selectables = selectables.Where(s => s != null && !s.Equals(null))?.ToList();
            if (selectables != null && selectables.Count > 0) {
                // dang select cac unit
                //print(selectables.Count + " selected");
                Vector2 mouseWorldPosition = InputExtension.MouseWorldPoint();
                foreach (ISelectable selectable in selectables) {
                    selectable.OnGiveOrder(mouseWorldPosition);
                }
                // ======= end remove
                ShowArrow(mouseWorldPosition);
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
        if (!isSelectingBox) {
            // neu chua bam chuot nao + dang k select
            if (Input.GetMouseButtonDown(2) && !InputExtension.IsMouseOverUI()){
                startMoveCameraPosition = InputExtension.MouseWorldPoint();
            }
            if (Input.GetMouseButton(2)){
                Vector3 currentMousePosition = InputExtension.MouseWorldPoint();
                Vector3 delta = startMoveCameraPosition - currentMousePosition;
                delta.z = 0;
                Camera.main.transform.position += delta;
            }
            if (Input.GetMouseButtonUp(2)){
            }
        }
  
        if (controlMoveCameraByEdgeScreen) {
            const int boundary = 10;
            int horizontal = 0;
            if (Input.mousePosition.x <= boundary) horizontal = -1;
            if (Input.mousePosition.x + boundary >= Screen.width) horizontal = 1;
            
            int vertical = 0;
            if (Input.mousePosition.y <= boundary) vertical = -1;
            if (Input.mousePosition.y + boundary >= Screen.height) vertical = 1;
            
            if (horizontal != 0 || vertical != 0) {
                Vector2 move = new Vector2(horizontal, vertical).normalized;
                Vector3 delta = move * Time.deltaTime * moveCamSpeed;
                delta.z = 0;
                Camera.main.transform.position += delta;
            }
        }
        

    }
}
