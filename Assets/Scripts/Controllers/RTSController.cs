using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RTSController : MonoBehaviour {
    [SerializeField] GameObject selectionBoxObj;
    [SerializeField] GameObject arrowMoveObj;
    [SerializeField] Tilemap tilemapGround;
    public bool controlMoveCameraByEdgeScreen = false;
    public float moveCamSpeed;
    List<UnitSelectable> selectedUnits = new List<UnitSelectable>();
    private BuildingSelectable onlyOneSelectedBuilding;
    //private bool isMultiSelect;
    Vector3 startSelectionBoxPosition;
    Vector3 startMoveCameraPosition;
    bool isSelectingBox = false;
    int moveCameraByMouse = -1;
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
                Collider2D[] collider2Ds = Physics2D.OverlapAreaAll(startSelectionBoxPosition, InputExtension.MouseWorldPoint());
                DeselectPrevDraggingSelectionBox();
                if (collider2Ds != null && collider2Ds.Length > 0) {
                    // TODO: kiem tra neu bam vao 1 phat thi mo control ui, neu k mo multiselect
                    if (collider2Ds.Length == 1) {
                        if (collider2Ds[0].TryGetComponent(out BuildingSelectable building)){
                            SelectOneBuilding(building);
                        }
                        else if (collider2Ds[0].TryGetComponent(out UnitSelectable unit)) {
                            SelectOneUnit(unit);
                        }
                    }
                    else if (collider2Ds.Length > 1) {
                        List<UnitSelectable> units = new List<UnitSelectable>();
                        foreach (Collider2D collider in collider2Ds){
                            if (collider.TryGetComponent(out UnitSelectable unit)){
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

        foreach (UnitSelectable prev in selectedUnits){
            if (prev != null){
                prev.OnDeselected?.Invoke();
                prev.OnShowControlUI?.Invoke(false);
            }
        }
        selectedUnits.Clear();
        if (onlyOneSelectedBuilding) {
            print("deselect building");
            onlyOneSelectedBuilding.OnDeselected();
            onlyOneSelectedBuilding.OnShowControlUI(false);
            onlyOneSelectedBuilding = null;
        }
    }
    // BUG
    private void SelectOneBuilding(BuildingSelectable building) {
        // on 1 building selected
        building.OnSelected();
        building.OnShowControlUI(true);
        onlyOneSelectedBuilding = building;
    }
    private void SelectOneUnit(UnitSelectable unit) {
        unit.OnSelected?.Invoke();
        unit.OnShowControlUI?.Invoke(true);
        selectedUnits.Add(unit);
    }
    private void SelectMoreThings(List<UnitSelectable> units) {
        if (units.Count == 1) {
            SelectOneUnit(units[0]);
        }
        else {
            foreach (UnitSelectable unit in units) {
                unit.OnSelected?.Invoke();
                selectedUnits.Add(unit);
                // no show control ui here
            }
        }
    }
    void GiveOrdersControl(){
        if (!isSelectingBox && Input.GetMouseButtonDown(1)){
            if (onlyOneSelectedBuilding) {
                // vua bam vao building sau do bam chuot ra cho khac => cha lam j ca
                print("deselected");
                onlyOneSelectedBuilding.OnDeselected();
                onlyOneSelectedBuilding.OnShowControlUI(false);
                onlyOneSelectedBuilding = null;
            }
            else if (selectedUnits != null && selectedUnits.Count > 0) {
                // dang select cac unit
                Vector2 mouseWorldPosition = InputExtension.MouseWorldPoint();
                RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPosition, Vector3.forward);
                if (hits != null && hits.Length > 0) {
                    foreach (RaycastHit2D hit in hits) {

                        // ========== THEM CAC LENH KHI BAM VAO VAT THE KHAC TAI DAY ===========
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
                        // bam vao enemy
                        if (hit.collider.TryGetComponent(out EnemyUnit enemyUnit)) {
                            //print("order");
                            GiveOrderAttackEnemy(enemyUnit);
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
        foreach (UnitSelectable selectable in selectedUnits){
            StopGathering(selectable);
            StopShooting(selectable);
            if (selectable.TryGetComponent(out UnitTaskGathering gathering)){
                gathering.SetBuildingStorage(storage);
                gathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToStorage);
                gathering.StartDoTask();
            }
        }
    }
    private void GiveOrderByResource(Resource resource) {
        foreach (UnitSelectable selectable in selectedUnits) {
            StopGathering(selectable);
            StopShooting(selectable);
            if (selectable.TryGetComponent(out UnitTaskGathering gathering)){
                gathering.SetResource(resource);
                gathering.SetState(UnitTaskGathering.TaskGatheringState.MoveToResource);
                gathering.StartDoTask();
            }
        }
    }
    private void GiveOrderAttackEnemy(EnemyUnit enemyUnit) {
        foreach (UnitSelectable selectable in selectedUnits) {
            StopGathering(selectable);
            if (selectable.TryGetComponent(out UnitShootable shootable)) {
                shootable.SetFollowEnemy(enemyUnit, UnitShootable.FollowEnemyState.Hunt);
            }
        }
    }
    private void GiveOrderByMoving(Vector2 position) {
        foreach (UnitSelectable selectable in selectedUnits){
            StopGathering(selectable);
            StopShooting(selectable);
            if (selectable.TryGetComponent(out UnitMovement movement)){
                movement.MoveToPosition(position);
            }
        }
    }
    private void StopGathering(UnitSelectable selectable) {
        if (selectable.TryGetComponent(out UnitTaskGathering gathering)) {
            gathering.EndDoTask();
        }
    }
    private void StopShooting(UnitSelectable selectable) {
        // tam dung tan cong
        if (selectable.TryGetComponent(out UnitShootable shootable)) {
            shootable.StopAttacking();
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
        if (moveCameraByMouse == -1 && !isSelectingBox) {
            // neu chua bam chuot nao + dang k select
            if (Input.GetMouseButtonDown(2) && !InputExtension.IsMouseOverUI()){
                startMoveCameraPosition = InputExtension.MouseWorldPoint();
                moveCameraByMouse = 2;
            }
        
            if (Input.GetMouseButtonDown(1) && !InputExtension.IsMouseOverUI()){
                startMoveCameraPosition = InputExtension.MouseWorldPoint();
                moveCameraByMouse = 1;
            }
        }

        if (moveCameraByMouse != -1) {
            // neu dang keo
            if (Input.GetMouseButton(moveCameraByMouse)){
                Vector3 currentMousePosition = InputExtension.MouseWorldPoint();
                Vector3 delta = startMoveCameraPosition - currentMousePosition;
                delta.z = 0;
                Camera.main.transform.position += delta;
            }
            if (Input.GetMouseButtonUp(moveCameraByMouse)){
                moveCameraByMouse = -1;
            }
        }
        else {
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
}
