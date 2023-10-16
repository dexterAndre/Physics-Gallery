using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController_RadioButton : GUIController
{
    public override void ApplyColorPalette(ColorPalette palette)
    {
        SetRadioButtonColors(transform.GetChild(0).GetComponent<Toggle>(), palette);
    }
}
