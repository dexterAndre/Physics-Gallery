using UnityEngine;
using UnityEngine.UI;



public abstract class GUIComponent : MonoBehaviour, IColorable
{
    [SerializeField] private Image interactableBackgroundIndicator;
    [SerializeField] protected Manager_GUI managerGUI;
    public Manager_GUI ManagerGUI { get { return managerGUI; } }
    [SerializeField] protected GUIOption_Header header;

    public Manager_GUI Manager_GUI { set { managerGUI = value; } }

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

    public virtual void ApplyColorPalette(ColorPalette palette)
    {
        if (header == null)
        {
            header = transform.GetChild(0).GetComponent<GUIOption_Header>();
            if (header == null)
            {
                return;
            }
        }

        header.ApplyColorPalette(palette);
    }
}
