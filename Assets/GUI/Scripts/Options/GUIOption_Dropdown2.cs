using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEditor;

public class GUIOption_Dropdown2 : GUIOption2, IColorable
{
    [SerializeField] private TMP_Dropdown dropdown;
    public TMP_Dropdown Dropdown { get { return dropdown; } }

    public int GetValue()
    {
        if (dropdown == null)
        {
            Debug.LogWarning("TMP_Dropdown \"dropdown\" is null. Cannot get value. Returning -1.");
            return -1;
        }

        return dropdown.value;
    }

    public override void SetInteractable(bool state)
    {
        dropdown.interactable = state;
    }

    public virtual void OverwriteDropdownEntries(List<string> entries)
    {
        dropdown.ClearOptions();
        foreach (string entry in entries)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(entry));
        }
        dropdown.RefreshShownValue();
        EditorUtility.SetDirty(dropdown);
    }
    public virtual void OverwriteDropdownEntries<T>(Dictionary<T, string> entries)
    {
        dropdown.ClearOptions();
        foreach (KeyValuePair<T, string> entry in entries)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(entry.Value));
        }
        dropdown.RefreshShownValue();
        EditorUtility.SetDirty(dropdown);
    }

    public virtual void ApplyColorPalette(ColorPalette palette)
    {
        IColorable.ApplyColorPalette_Dropdown(Dropdown, palette);
    }
}
