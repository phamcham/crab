using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIE_PauseGame : MonoBehaviour {
    [SerializeField] Image holder;
    [SerializeField] Button resumeGameButton;
    private void Awake() {
        resumeGameButton.onClick.AddListener(() => {
            GameController.current.ResumeGame();
        });
    }
    public void ClosePauseWindowUI() {
        holder.gameObject.SetActive(false);
    }
    public void OpenPauseWindowUI() {
        holder.gameObject.SetActive(true);
    }
}
