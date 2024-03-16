using UnityEngine;



public class GUIComponent_Generate2 : GUIComponent2, IPopulatable
{
    [SerializeField] private GUIOption_IncrementalSlider2 controllerPointCount;
    [SerializeField] private GUIOption_Dropdown2 controllerMethod;
    [SerializeField] private GUIOption_Buttons2 controllerButtons_ClearGenerate;

    public int PointCount { get { return Mathf.RoundToInt(controllerPointCount.GetValue()); } }
    public int Method { get { return controllerMethod.GetValue(); } }



    protected override void CheckReferences()
    {
        if (controllerPointCount == null ||
            controllerMethod == null)
        {
            Debug.LogWarning("GUI Options null. Cannot query GUI Component state.");
            return;
        }
    }

    public override void SetInteractable(bool state)
    {
        base.SetInteractable(state);
        controllerPointCount.SetInteractable(state);
        controllerMethod.SetInteractable(state);
        controllerButtons_ClearGenerate.SetInteractable(state);
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);
        controllerPointCount.ApplyColorPalette(palette);
        controllerMethod.ApplyColorPalette(palette);
        controllerButtons_ClearGenerate.ApplyColorPalette(palette);
    }

    public void Populate()
    {
        IPopulatable.Populate_Dropdown<BehaviorMethod>(controllerMethod.Dropdown, managerGUI.NameList_GenerationMethods);
    }
}
