using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputExtension
{
    public static bool IsMouseOverUI(int pointerId = 0){
        return EventSystem.current.IsPointerOverGameObject(pointerId);
    }
    public static Vector2 ScreenToWorldPoint(Vector2 position){
        return Camera.main.ScreenToWorldPoint(position);
    }
    public static Vector2 MouseWorldPoint(){
        return ScreenToWorldPoint(Input.mousePosition);
    }
}
