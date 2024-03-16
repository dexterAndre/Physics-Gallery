

public class GUIOption_ToggleFlip : GUIOption_Toggle
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
