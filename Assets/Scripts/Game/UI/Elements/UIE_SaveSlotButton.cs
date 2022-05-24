using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIE_SaveSlotButton : MonoBehaviour {
    [SerializeField] private TMPro.TextMeshProUGUI text;
    SlotContainerData.SlotData data;
    public void Setup(SlotContainerData.SlotData data) {
        this.data = data;
        text.text = data.info;
    }

    public void OnLoad() {
        SaveSlotManager.current.LoadSlot(data);
    }
    public void OnClear() {
        SaveSlotManager.current.ClearSlot(data);
    }
}
