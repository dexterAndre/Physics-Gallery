using UnityEngine;

public class GUIOption_DropdownGallery : GUIOption_Dropdown
{
    [SerializeField] private GUIController_DropdownGallery dropdownGallery;

    public override void SetInteractable(bool state)
    {
        dropdownGallery.Dropdown.interactable = state;
        dropdownGallery.ButtonDecrement.interactable = state;
        dropdownGallery.ButtonIncrement.interactable = state;
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);
    }

    public override void ApplyDropdownPalette(ColorPalette palette)
    {
        IColorable.ApplyColorPalette_DropdownGallery(dropdownGallery, palette);
    }
}
