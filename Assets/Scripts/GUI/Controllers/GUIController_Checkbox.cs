using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController_Checkbox : GUIController
{
    public override void ApplyColorPalette(ColorPalette palette)
    {
        SetToggleColors(transform.GetChild(1).GetChild(0).GetComponent<Toggle>(), palette);
    }
}
