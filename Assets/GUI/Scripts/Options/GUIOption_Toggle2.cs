using UnityEngine;
using UnityEngine.UI;

public class GUIOption_Toggle2 : GUIOption2
{
    [SerializeField] protected Toggle toggle;
    public Toggle Toggle { get { return toggle; } }

    public bool GetValue()
    {
        if (toggle == null)
        {
            Debug.LogWarning("Toggle \"toggle\" is null. Cannot get value. Returning false.");
            return false;
        }

        return toggle.isOn;
    }

    public override void SetInteractable(bool state)
    {
        toggle.interactable = state;
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);
        ApplyTogglePalette(palette);
    }

    public virtual void ApplyTogglePalette(ColorPalette palette)
    {
        IColorable.ApplyColorPalette_Toggle(toggle, palette);
    }
}
