using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIOption_ToggleFlip2 : GUIOption_Toggle2
{
    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);
    }

    public override void ApplyTogglePalette(ColorPalette palette)
    {
        IColorable.ApplyColorPalette_FlipToggle(toggle, palette);
    }
}
