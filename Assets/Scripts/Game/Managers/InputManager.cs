using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputExtension {
    private static List<RaycastResult> _raycastUIResult = new List<RaycastResult>();
    public static bool IsMouseOverUI(){
        if (EventSystem.current.IsPointerOverGameObject()){
            return true;
        }
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        _raycastUIResult.Clear();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, _raycastUIResult);
        return _raycastUIResult.Count > 0;
    }
    public static Vector2 ScreenToWorldPoint(Vector2 position){
        return Camera.main.ScreenToWorldPoint(position);
    }
    public static Vector2 MouseWorldPoint(){
        return ScreenToWorldPoint(Input.mousePosition);
    }
}
