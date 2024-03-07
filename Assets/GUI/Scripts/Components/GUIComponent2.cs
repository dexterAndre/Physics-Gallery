using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GUIComponent2 : MonoBehaviour
{
    [SerializeField] private Image interactableBackgroundIndicator;
    [SerializeField] protected ManagerGUI managerGUI;

    public ManagerGUI Manager_GUI { set { managerGUI = value; } }

    protected abstract void CheckReferences();
    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    public virtual void SetInteractable(bool state)
    {
        interactableBackgroundIndicator.enabled = !state;
    }
    public void DeleteComponent()
    {
        managerGUI.RemoveComponent(gameObject);
    }
}
