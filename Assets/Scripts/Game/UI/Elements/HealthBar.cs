using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    [SerializeField] Transform bar;
    bool isShowing = false;

    public void SetSize(float sizeNomalized) {
        sizeNomalized = Mathf.Clamp01(sizeNomalized);
        bar.localScale = new Vector3(sizeNomalized, 1f, 1f);

        gameObject.SetActive(true);
        if (!isShowing) {
            StopCoroutine(nameof(AutoHide));
            StartCoroutine(nameof(AutoHide));
        }
    }
    IEnumerator AutoHide() {
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }

    public void Show() {
        isShowing = true;
        gameObject.SetActive(true);
    }
    public void Hide() {
        isShowing = false;
        gameObject.SetActive(false);
    }
}
