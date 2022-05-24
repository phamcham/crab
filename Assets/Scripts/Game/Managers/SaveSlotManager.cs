using System.Collections;
using System.Collections.Generic;
using PhamCham.Extension;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotManager : MonoBehaviour {
    public static SaveSlotManager current;
    [SerializeField] UIE_SaveSlotButton saveSlotButtonPrefab;

    private const string SLOT_DATA_PATH = "slotData.txt";
    List<UIE_SaveSlotButton> uis = new List<UIE_SaveSlotButton>();
    private void Awake() {
        current = this;
    }
    private void Start() {
        saveSlotButtonPrefab.gameObject.SetActive(false);
    }

    private void OnEnable() {
        RefreshList();
    }

    private void RefreshList() {
        foreach (UIE_SaveSlotButton ui in uis) {
            Destroy(ui.gameObject);
        }
        uis.Clear();

        if (TryGetSlotContainerData(out SlotContainerData container)) {
            foreach (SlotContainerData.SlotData saveSlot in container.slotDatas) {
                UIE_SaveSlotButton ui = Instantiate(saveSlotButtonPrefab.gameObject, transform).GetComponent<UIE_SaveSlotButton>();
                ui.gameObject.SetActive(true);
                ui.Setup(saveSlot);

                uis.Add(ui);
            }
        }
        else {
            // empty
        }
    }
    private bool TryGetSlotContainerData(out SlotContainerData container) {
        return SaveSystem.LoadJson(out container, SLOT_DATA_PATH) && container != null && container.slotDatas != null && container.slotDatas.Count > 0;
    }

    public void ClearSlot(SlotContainerData.SlotData slotData) {
        if (TryGetSlotContainerData(out SlotContainerData container)) {
            container.slotDatas.Remove(slotData);
            RefreshList();
        }
    }

    public void LoadSlot(SlotContainerData.SlotData slotData) {
        // TODO: mo scene load map here
        SceneManager.LoadScene("GameplayScene");
    }


    public void OnCreateNewSlot() {
        print("OnCreateNewSlot");
        // TODO: mo scene load new map
        SceneManager.LoadScene("GameplayScene");
    }
}

[System.Serializable]
public class SlotContainerData {
    public List<SlotData> slotDatas;

    [System.Serializable]
    public class SlotData {
        public string info;
    }
}