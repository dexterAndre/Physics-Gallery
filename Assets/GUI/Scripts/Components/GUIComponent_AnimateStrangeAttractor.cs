using UnityEngine;

public class GUIComponent_AnimationStrangeAttractor : GUIComponent, IPopulatable
{
    [SerializeField] private GUIOption_IncrementalSlider controllerSpeed;
    [SerializeField] private GUIOption_Dropdown controllerType;

    public float Speed { get { return controllerSpeed.GetValue(); } }
    public StrangeAttractorType Type { get { return (StrangeAttractorType)controllerType.GetValue(); } }



    protected override void CheckReferences()
    {
        if (controllerSpeed == null ||
            controllerType == null)
        {
            Debug.LogWarning("GUI Options null. Cannot query GUI Component state.");
            return;
        }
    }

    public override void SetInteractable(bool state)
    {
        base.SetInteractable(state);
        controllerSpeed.SetInteractable(state);
        controllerType.SetInteractable(state);
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);
        // TODO: Implement
    }

    public void Populate()
    {
        // TODO: Only strange attractors
        //IPopulatable.Populate_Dropdown<BehaviorMethod>(controllerType.Dropdown, managerGUI.NameList_Components);
    }
}
