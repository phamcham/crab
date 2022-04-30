using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelectable : MonoBehaviour {
    public Building BaseBuilding { get; private set; }
    private void Awake() {
        BaseBuilding = GetComponent<Building>();
    }

    public Action OnSelected;
    public Action OnDeselected;
    public Action<bool> OnShowControlUI;
}
