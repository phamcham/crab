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
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, Input.mousePosition, mainCamera, out Vector3 worldPoint)) {
                mainCamera.transform.position = worldPoint;
            }
        }

        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;
        viewMinimap.localScale = new Vector3(width, height);
    }

}
