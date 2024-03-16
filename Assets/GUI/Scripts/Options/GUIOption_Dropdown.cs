using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;



public class GUIOption_Dropdown : GUIOption
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

    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);
        ApplyDropdownPalette(palette);
    }

    public virtual void ApplyDropdownPalette(ColorPalette palette)
    {
        IColorable.ApplyColorPalette_Dropdown(Dropdown, palette);
    }

    public static void Populate_Dropdown<T>(TMP_Dropdown dropdown, Dictionary<T, string> nameList)
    {
        dropdown.ClearOptions();
        List<string> names = nameList.Values.ToList();
        dropdown.AddOptions(names);
        dropdown.RefreshShownValue();
        EditorUtility.SetDirty(dropdown);
    }
}
