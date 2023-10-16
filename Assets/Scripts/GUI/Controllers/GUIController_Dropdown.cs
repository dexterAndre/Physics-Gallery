using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIController_Dropdown : GUIController
{
    [SerializeField] private TMP_Dropdown dropdown;
    public int Value
    {
        get
        {
            return dropdown.value;
        }
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        SetDropdownColors(transform.GetChild(1).GetComponent<TMP_Dropdown>(), palette);
    }
}
