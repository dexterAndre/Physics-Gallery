using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIOption_Buttons2 : GUIOption2, IColorable
{
    [SerializeField] private List<Button> buttons;
    public List<Button> Buttons { get { return buttons; } }

    public override void SetInteractable(bool state)
    {
        foreach (Button button in buttons)
        {
            button.interactable = state;
        }
    }

    public void ApplyColorPalette(ColorPalette palette)
    {
        foreach (Button button in buttons)
        {
            IColorable.ApplyColorPalette_Button(button, palette);
        }
    }
}
