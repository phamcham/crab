using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionOneUIControlManager : MonoBehaviour {
    public static SelectionOneUIControlManager current { get; private set; }
    [SerializeField] Transform oneSelectionHolder;
    private void Awake() {
        current = this;
    }
    public void AddControlUI(UIE_UIControl control) {
        control.transform.SetParent(oneSelectionHolder);
    }
    public Transform GetHolder(){
        return oneSelectionHolder;
    }
}
