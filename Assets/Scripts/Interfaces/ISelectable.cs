using UnityEngine;

public interface ISelectable {
    public void OnSelected();
    public void OnDeselected();
    public void OnGiveOrder(Vector2 position);
    public void OnShowControlUI(bool isShow);
}