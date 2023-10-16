using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(GUIManager))]
public class GUIManager_Editor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        GUIManager manager = (GUIManager)target;
        VisualElement root = new VisualElement();
        var inspector = new IMGUIContainer(() => base.OnInspectorGUI());
        root.Add(inspector);

        Action applyColorPaletteAction = () => manager.ApplyColorPalette();
        Action applyRandomColorPaletteAction = () => manager.ApplyRandomColorPalette();
        Button applyColorsButton = new Button(applyColorPaletteAction);
        applyColorsButton.text = "Apply Color Scheme";
        Button applyRandomColorsButton = new Button(applyRandomColorPaletteAction);
        applyRandomColorsButton.text = "Apply Random Color Scheme";
        root.Add(applyColorsButton);
        root.Add(applyRandomColorsButton);

        return root;
    }
}
