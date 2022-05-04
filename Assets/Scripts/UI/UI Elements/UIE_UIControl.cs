using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIE_UIControl : MonoBehaviour {
    float updateInterval = 0.2f;
    float curUpdateUIInterval;
    protected void OnEnable() {
        curUpdateUIInterval = 0;
    }
    protected void Update() {
        if (curUpdateUIInterval <= 0) {
            curUpdateUIInterval = updateInterval;
            UpdateIntervalOnUI();
        }
        else {
            curUpdateUIInterval -= Time.deltaTime;
        }
    }
    protected abstract void UpdateIntervalOnUI();
    public void Show() {
        gameObject.SetActive(true);
    }
    
    public void Hide() {
        gameObject.SetActive(false);
    }

}
