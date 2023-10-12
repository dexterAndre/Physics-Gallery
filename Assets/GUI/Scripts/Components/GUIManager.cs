using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ColorPalette
{
    public Color colorBackgroundFill;
    public Color colorForegroundFill;
    public Color colorBackgroundPanel;
    public Color colorFocusFill;
    public Color colorFocusStroke;
    public Color colorFocusGlyph;
    public Color colorClickableNormal;
    public Color colorClickableHighlighted;
    public Color colorClickablePressed;
    public Color colorClickableSelected;
    public Color colorClickableDisabled;
    public Color colorFont;
}

public class GUIManager : MonoBehaviour
{
    [SerializeField] private Image menuBorder;
    [SerializeField] private Image menuBackground;
    [SerializeField] private List<GUIComponent> components;
    [SerializeField] private ColorPalette colorPalette;

    public void ApplyColorPalette(ColorPalette palette)
    {
        menuBorder.color = palette.colorForegroundFill;
        EditorUtility.SetDirty(menuBorder);
        menuBackground.color = palette.colorBackgroundPanel;
        EditorUtility.SetDirty(menuBackground);
        foreach (GUIComponent component in components)
        {
            component.ApplyColorPalette(palette);
        }
    }
    public void ApplyColorPalette()
    {
        ApplyColorPalette(colorPalette);
    }
    public void ApplyRandomColorPalette()
    {
        ColorPalette palette = new ColorPalette();
        palette.colorBackgroundFill = Random.ColorHSV();
        palette.colorForegroundFill = Random.ColorHSV();
        palette.colorFocusFill = Random.ColorHSV();
        palette.colorFocusStroke = Random.ColorHSV();
        palette.colorFocusGlyph = Random.ColorHSV();
        palette.colorClickableNormal = Random.ColorHSV();
        palette.colorClickableHighlighted = Random.ColorHSV();
        palette.colorClickablePressed = Random.ColorHSV();
        palette.colorClickableSelected = Random.ColorHSV();
        palette.colorClickableDisabled = Random.ColorHSV();
        palette.colorFont = Random.ColorHSV();
        ApplyColorPalette(palette);
    }
}
