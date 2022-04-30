using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    [SerializeField] Transform bar;

    // sizeNomalized = curHeath / maxHeath
    public void SetSize(float sizeNomalized) {
        sizeNomalized = Mathf.Clamp01(sizeNomalized);
        bar.localScale = new Vector3(sizeNomalized, 1f, 1f);
    }
}
