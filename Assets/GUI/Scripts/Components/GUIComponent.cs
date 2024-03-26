using UnityEngine;
using UnityEngine.UI;



public abstract class GUIComponent : MonoBehaviour, IColorable
{
    [SerializeField] private Image interactableBackgroundIndicator;
    [SerializeField] protected GUIOption_Header header;



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
        Manager_Lookup.Instance.ManagerGUI.RemoveComponent(gameObject);
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
