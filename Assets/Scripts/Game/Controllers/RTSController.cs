using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RTSController : MonoBehaviour {
    [Space, Header("Selection Box")]
    [SerializeField] GameObject selectionBoxObj;
    [SerializeField] GameObject arrowMoveObj;
    [SerializeField] Tilemap tilemapGround;
    [Space, Header("Camera move edge"), Space]
    public bool controlMoveCameraByEdgeScreen = false;
    public float moveCamSpeed;
    float curDelayMoveCamTime = 0;
    List<ISelectable> selectables = new List<ISelectable>();
    //private bool isMultiSelect;
    Vector3 startSelectionBoxPosition;
    Vector3 startMoveCameraPosition;
    bool isSelectingBox = false;
    bool multiSelect = true;
    List<PlayerUnit> selectedPlayerUnits = new List<PlayerUnit>();
    PlayerUnit selectedUnit = null;
    Building selectedBuilding = null;
    Resource selectedResource = null;

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

        // setting cursor
        Cursor.visible = false;
    }

    void SelectionBoxControl(){
        if (Input.GetMouseButtonDown(0) && !InputExtension.IsMouseOverUI()){
            StartDraggingSelectionBox();
        }
        if (isSelectingBox){
            if (Input.GetMouseButton(0)) {
                DraggingSelectionBox();
                if (!Input.GetKey(KeyCode.LeftControl)) {
                    // neu k giu control tu dau den cuoi thi khong multiselect
                    multiSelect = false;
                }
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
        foreach (ISelectable prev in selectables){
            if (prev != null && !prev.Equals(null)) {
                prev.OnDeselected();
                prev.OnShowControlUI(false);
            }
        }
    }
    private void SelecteDraggingSelectionBox(Vector2 pointA, Vector2 pointB) {
        if (!multiSelect) selectables.Clear();
        
        Collider2D[] collider2Ds = Vector2.Distance(pointA, pointB) < 1
                                ? new Collider2D[] { Physics2D.OverlapArea(pointA, pointB) }
                                : Physics2D.OverlapAreaAll(pointA, pointB);
                
        if (collider2Ds != null && collider2Ds.Length > 0) {
            // TODO: kiem tra neu bam vao 1 phat thi mo control ui, neu k mo multiselect
            // Check all units selected
            if (!multiSelect) {
                selectedPlayerUnits = new List<PlayerUnit>();
                selectedUnit = null;
                selectedBuilding = null;
                selectedResource = null;
            }
            foreach (Collider2D collider in collider2Ds) {
                if (collider == null || collider.Equals(null)) continue;
                if (collider.TryGetComponent(out ISelectable selectable)) {
                    if (collider.TryGetComponent(out PlayerUnit unit)) {
                        if (!selectedUnit) {
                            selectedUnit = unit;
                        }
                        // neu count > 1  =>  chon nhieu unit
                        selectedPlayerUnits.Add(unit);
                        
                    }
                    else if (!selectedBuilding && collider.TryGetComponent(out Building building)) {
                        selectedBuilding = building;
                    }
                    else if (!selectedResource && collider.TryGetComponent(out Resource resource)) {
                        selectedResource = resource;
                    }
                }
            }

            if (selectedPlayerUnits.Count > 1) {
                // chon nhieu unit thi k show ui control
                foreach (ISelectable unit in selectedPlayerUnits) {
                    unit.OnSelected();
                    selectables.Add(unit);
                }
            }
            else {
                ISelectable selectable = null;
                // if else theo thu tu uu tien
                if (selectedUnit) selectable = selectedUnit as ISelectable;
                else if (selectedBuilding) selectable = selectedBuilding as ISelectable;
                else if (selectedResource) selectable = selectedResource as ISelectable;

                if (selectable != null && !selectable.Equals(null)) {
                    selectable.OnSelected();
                    selectable.OnShowControlUI(true);
                    //print("show ui");
                    selectables.Add(selectable);
                }
            }
        }
        // reset
        multiSelect = true;
        isSelectingBox = false;
        selectionBoxObj.SetActive(false);
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
                    ITakeOrder takeOrder = selectable as ITakeOrder;
                    if (takeOrder != null && !takeOrder.Equals(null)) {
                        takeOrder.OnTakeOrderAtPosition(mouseWorldPosition);
                    }
                }
                
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
            //.OnComplete(() => DOVirtual.DelayedCall(3f, () => arrowMoveObj.SetActive(false)).Play())
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
                
                Vector3 newCamPos = Camera.main.transform.position + delta;

                newCamPos.x = Mathf.Clamp(newCamPos.x, tilemapGround.cellBounds.xMin, tilemapGround.cellBounds.xMax);
                newCamPos.y = Mathf.Clamp(newCamPos.y, tilemapGround.cellBounds.yMin, tilemapGround.cellBounds.yMax); 

                Camera.main.transform.position = newCamPos;

                GameCursor.SetCursorSpriteOnFrame(GameCursor.CursorState.Move);
            }
            if (Input.GetMouseButtonUp(2)){
            }
        }
  
        if (controlMoveCameraByEdgeScreen) {
            const int boundary = 10;
            int horizontal = 0;
            if (Input.mousePosition.x <= boundary) {
                horizontal = -1;
                GameCursor.SetCursorSpriteOnFrame(GameCursor.CursorState.Left);
            }
            if (Input.mousePosition.x + boundary >= Screen.width) {
                horizontal = 1;
                GameCursor.SetCursorSpriteOnFrame(GameCursor.CursorState.Right);
            }
            
            int vertical = 0;
            if (Input.mousePosition.y <= boundary) {
                vertical = -1;
                if (horizontal == -1) {
                    GameCursor.SetCursorSpriteOnFrame(GameCursor.CursorState.DownLeft);
                }
                else if (horizontal == 1) {
                    GameCursor.SetCursorSpriteOnFrame(GameCursor.CursorState.DownRight);
                }
                else {
                    GameCursor.SetCursorSpriteOnFrame(GameCursor.CursorState.Down);
                }
            }
            if (Input.mousePosition.y + boundary >= Screen.height) {
                vertical = 1;
                if (horizontal == -1) {
                    GameCursor.SetCursorSpriteOnFrame(GameCursor.CursorState.UpLeft);
                }
                else if (horizontal == 1) {
                    GameCursor.SetCursorSpriteOnFrame(GameCursor.CursorState.UpRight);
                }
                else {
                    GameCursor.SetCursorSpriteOnFrame(GameCursor.CursorState.Up);
                }
            }

            const float delayMoveCamTime = 0.1f;
            if (horizontal != 0 || vertical != 0) {
                if (curDelayMoveCamTime >= delayMoveCamTime) {
                    Vector2 move = new Vector2(horizontal, vertical).normalized;
                    Vector3 delta = move * Time.deltaTime * moveCamSpeed;
                    delta.z = 0;

                    Vector3 newCamPos = Camera.main.transform.position + delta;

                    newCamPos.x = Mathf.Clamp(newCamPos.x, tilemapGround.cellBounds.xMin, tilemapGround.cellBounds.xMax);
                    newCamPos.y = Mathf.Clamp(newCamPos.y, tilemapGround.cellBounds.yMin, tilemapGround.cellBounds.yMax); 

                    Camera.main.transform.position = newCamPos;
                }
                else {
                    curDelayMoveCamTime += Time.deltaTime;
                }
            }
            else {
                curDelayMoveCamTime = 0;
            }
        }
        

    }
}
