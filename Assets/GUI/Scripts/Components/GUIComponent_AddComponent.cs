using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIComponent_AddComponent : GUIComponent, IPopulatable
{
    [SerializeField] private Image separatorBar;
    [SerializeField] private GUIOption_Dropdown controllerAddComponent;

    public void SelectDropdownEntry(int index)
    {
        // Decrementing index by one to accommodate for the topmost "Cancel" option
        Manager_Lookup.Instance.ManagerGUI.AddComponent((BehaviorMethod)(index - 1));
        controllerAddComponent.Dropdown.SetValueWithoutNotify(0);
    }

    protected override void CheckReferences()
    {
        if (controllerAddComponent == null)
        {
            Debug.LogWarning("GUI Options null. Cannot query GUI Component state.");
            return;
        }
        if (Manager_Lookup.Instance.ManagerGUI == null)
        {
            Debug.LogWarning("GUI Options null. Cannot query GUI Component state.");
            return;
        }
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);

        Image strokeImage = controllerAddComponent.transform.GetChild(0).GetComponent<Image>();
        Image fillImage = strokeImage.transform.GetChild(0).GetComponent<Image>();
        TMP_Text label = fillImage.transform.GetChild(0).GetComponent<TMP_Text>();

        controllerAddComponent.ApplyColorPalette(palette);
        IColorable.ApplyColorPalette_StrokeFillGlyphLabel(palette, strokeImage, fillImage, null, label);
    }

    public void Populate()
    {
        // Adding a topmost "Cancel" option. Remember to subtract any indices by 1 when using the onValueChanged callback for this.
        // Whenever anything is selected, it is set back to index 0.
        // This is because the onValueChanged callback only reacts when the value is changed, and setting invalid values causes other issues.
        List<string> options = new List<string>() { "Cancel" };
        options.AddRange(Manager_Lookup.Instance.ManagerGUI.NameList_Components.Values);
        IPopulatable.Populate_Dropdown(controllerAddComponent.Dropdown, options);
    }
}
