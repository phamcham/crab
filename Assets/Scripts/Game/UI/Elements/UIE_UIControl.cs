using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class UIE_UIControl : MonoBehaviour {
    private Tween updateIntervalOnUITween;
    protected virtual void Start() {
        UpdateIntervalOnUI();
        updateIntervalOnUITween = DOVirtual.DelayedCall(0.1f, UpdateIntervalOnUI).SetLoops(-1).Play();
    }
    protected abstract void UpdateIntervalOnUI();
    public void Show() {
        gameObject.SetActive(true);
    }
    
    public void Hide() {
        gameObject.SetActive(false);
    }
    private void OnDestroy() {
        updateIntervalOnUITween.Kill();
    }
}
