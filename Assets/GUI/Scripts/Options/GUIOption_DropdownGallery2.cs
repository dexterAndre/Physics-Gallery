using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class GUIOption_DropdownGallery2 : GUIOption_Dropdown2, IColorable
{
    [SerializeField] private GUIController_DropdownGallery dropdownGallery;

    public override void SetInteractable(bool state)
    {
        dropdownGallery.Dropdown.interactable = state;
        dropdownGallery.ButtonDecrement.interactable = state;
        dropdownGallery.ButtonIncrement.interactable = state;
    }

    public override void OverwriteDropdownEntries(List<string> entries)
    {
        dropdownGallery.Dropdown.ClearOptions();
        foreach (string entry in entries)
        {
            dropdownGallery.Dropdown.options.Add(new TMP_Dropdown.OptionData(entry));
        }
        dropdownGallery.Dropdown.RefreshShownValue();
        EditorUtility.SetDirty(dropdownGallery.Dropdown);
    }

    public override void OverwriteDropdownEntries<T>(Dictionary<T, string> entries)
    {
        dropdownGallery.Dropdown.ClearOptions();
        foreach (KeyValuePair<T, string> entry in entries)
        {
            dropdownGallery.Dropdown.options.Add(new TMP_Dropdown.OptionData(entry.Value));
        }
        dropdownGallery.Dropdown.RefreshShownValue();
        EditorUtility.SetDirty(dropdownGallery.Dropdown);
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        IColorable.ApplyColorPalette_DropdownGallery(dropdownGallery, palette);
    }
}
