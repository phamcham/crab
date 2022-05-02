using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectable : MonoBehaviour {
    public Unit BaseUnit { get; private set; }
    private void Awake() {
        BaseUnit = GetComponent<Unit>();
    }

    public Action OnSelected;
    public Action OnDeselected;
    public Action<bool> OnShowControlUI;
}
