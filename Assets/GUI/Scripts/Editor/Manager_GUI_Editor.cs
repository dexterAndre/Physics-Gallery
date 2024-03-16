using System;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(Manager_GUI))]
public class Manager_GUI_Editor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        Manager_GUI manager = (Manager_GUI)target;
        VisualElement root = new VisualElement();
        var inspector = new IMGUIContainer(() => base.OnInspectorGUI());
        root.Add(inspector);

        Action applyColorPaletteAction = () => manager.ApplyColorPalette(manager.Palette);
        Button applyColorsButton = new Button(applyColorPaletteAction);
        applyColorsButton.text = "Apply Color Scheme";

        Action applyRandomColorPaletteAction = () => manager.ApplyColorPalette(ColorPalette.RandomPalette(manager.Palette));
        Button applyRandomColorsButton = new Button(applyRandomColorPaletteAction);
        applyRandomColorsButton.text = "Apply Random Color Scheme";

        Action applyDropdownItemsACtion = () => manager.PopulateDropdowns();
        Button applyDropdownItemsButton = new Button(applyDropdownItemsACtion);
        applyDropdownItemsButton.text = "Apply Dropdown Items";

        root.Add(applyColorsButton);
        root.Add(applyRandomColorsButton);
        root.Add(applyDropdownItemsButton);

        return root;
    }
}
