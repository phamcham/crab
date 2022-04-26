using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitSelectable : MonoBehaviour {
    [SerializeField] GameObject selectorObj;
    private void Awake() {
        selectorObj.SetActive(false);
    }
    public void OnSelected(){
        selectorObj.SetActive(true);
    }
    public void OnDeselected(){
        selectorObj.SetActive(false);
    }
    public void OnTakeOrder(){

    }
}
