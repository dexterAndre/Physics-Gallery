using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIController_Dropdown : GUIController
{
    public override void ApplyColorPalette(ColorPalette palette)
    {
        SetDropdownColors(transform.GetChild(1).GetComponent<TMP_Dropdown>(), palette);
    }
}
