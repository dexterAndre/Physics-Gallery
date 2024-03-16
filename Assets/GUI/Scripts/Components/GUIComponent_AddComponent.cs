using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIComponent_AddComponent : GUIComponent, IPopulatable
{
    [SerializeField] private Image separatorBar;
    [SerializeField] private GUIOption_Dropdown controllerAddComponent;

    public void SelectDropdownEntry(int index)
    {
        // TODO: Temp
        if (index == 1)
            managerGUI.AddComponent(BehaviorMethod.Animate_Jitter);
        else if (index == 2)
            managerGUI.AddComponent(BehaviorMethod.Animate_StrangeAttractor);
        controllerAddComponent.Dropdown.SetValueWithoutNotify(0);
    }

    protected override void CheckReferences()
    {
        if (controllerAddComponent == null)
        {
            Debug.LogWarning("GUI Options null. Cannot query GUI Component state.");
            return;
        }
        if (managerGUI == null)
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
        IPopulatable.Populate_Dropdown<BehaviorMethod>(controllerAddComponent.Dropdown, managerGUI.NameList_Components);
    }

}
