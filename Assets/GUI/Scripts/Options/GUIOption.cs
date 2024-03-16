using UnityEngine;
using TMPro;

public abstract class GUIOption : MonoBehaviour, IColorable
{
    [SerializeField] protected TMP_Text descriptor;

    public virtual void ApplyColorPalette(ColorPalette palette)
    {
        if (descriptor != null)
        {
            IColorable.ApplyColorPalette_Label(descriptor, palette.colorFont);
        }
    }

    public abstract void SetInteractable(bool state);
}
