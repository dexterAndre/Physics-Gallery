using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController_Header : GUIController
{
    public override void ApplyColorPalette(ColorPalette palette)
    {
        SetHeaderColors(GetComponent<Image>(), palette);
    }
}
