using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour {
    [SerializeField] Image fill;
    public void SetSize(float sizeNormalize) {
        sizeNormalize = Mathf.Clamp01(sizeNormalize);
        fill.fillAmount = sizeNormalize;
    }
    public void SetColor(Color color) {
        fill.color = color;
    }
}
