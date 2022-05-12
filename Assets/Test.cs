using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TMPro.TMP_FontAsset font;

    [ContextMenu("execute")]
    private void execute() {
        var list = FindObjectsOfType<TMPro.TextMeshProUGUI>();
        foreach (var element in list) {
            if (element.font != font) element.fontSizeMax += 5;
            element.font = font;
        }
    }
}
