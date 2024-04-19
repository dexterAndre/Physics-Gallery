using System.Linq;
using UnityEngine;
using static BehaviorSpecifications;
using static Manager_PointSet;



public class GUIComponent_Generate : GUIComponent, IPopulatable
{
    [SerializeField] private GUIOption_IncrementalSlider controllerPointCount;
    [SerializeField] private GUIOption_Dropdown controllerMethod;
    [SerializeField] private GUIOption_Buttons controllerButtons_ClearGenerate;

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
        // TODO: Decide when to use 2D vs. 3D version
        IPopulatable.Populate_Dropdown(controllerMethod.Dropdown, BehaviorSpecifications.BehaviorSpecification<VoidDelegate_ZeroParameters>.MethodNames<GenerationMethod, VoidDelegate_ZeroParameters>(BehaviorSpecifications.GenerationMethods, true));
    }
}
