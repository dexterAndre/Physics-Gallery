using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class GUIOption_DropdownGallery2 : GUIOption_Dropdown2
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
