using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController_FlipToggle : GUIController
{
    public override void ApplyColorPalette(ColorPalette palette)
    {
        SetFlipToggleColors(transform.GetChild(1).GetChild(0).GetComponent<Button>(), palette);
    }
}
