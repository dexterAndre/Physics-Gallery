using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController_Button : GUIController
{
    public override void ApplyColorPalette(ColorPalette palette)
    {
        SetButtonColors(transform.GetChild(0).GetComponent<Button>(), palette);
    }
}
