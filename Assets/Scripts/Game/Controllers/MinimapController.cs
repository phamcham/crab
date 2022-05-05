using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MinimapController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] Camera minimapCamera;
    [SerializeField] Camera mainCamera;
    [SerializeField] Tilemap terrainTilemap;
    [SerializeField] Transform viewMinimap;

    private RectTransform _rectTransform;
    RectTransform rectTransform {
        get {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    Vector2 topRight, bottomLeft;
    bool isDragging = false;

    public void OnPointerDown(PointerEventData eventData) {
        isDragging = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
    private void Awake() {
        terrainTilemap.CompressBounds();

        topRight = (Vector3)terrainTilemap.cellBounds.max;
        bottomLeft = (Vector3)terrainTilemap.cellBounds.min;

        Vector3 center = terrainTilemap.cellBounds.center;
        center.z = minimapCamera.transform.position.z;

        minimapCamera.orthographicSize = Mathf.Max(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y) / 2;
        minimapCamera.transform.position = center;
    }

    private void Update() {
        if (isDragging) {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out Vector2 localPoint)) {
                Vector2 rectSize = rectTransform.rect.size;
                localPoint += rectSize / 2;
                //print(localPoint);
                Vector2 ratio = localPoint / rectSize;

                float worldX = Mathf.Lerp(bottomLeft.x, topRight.x, ratio.x);
                float worldY = Mathf.Lerp(bottomLeft.y, topRight.y, ratio.y);

                Vector3 world = new Vector3(worldX, worldY, mainCamera.transform.position.z);
                mainCamera.transform.position = world;
            }
        }

        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;
        viewMinimap.localScale = new Vector3(width, height);
    }

}
