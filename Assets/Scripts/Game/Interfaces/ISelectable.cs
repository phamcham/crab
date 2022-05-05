using UnityEngine;

public interface ISelectable {
    public void OnSelected();
    public void OnDeselected();
    public void OnShowControlUI(bool isShow);
}