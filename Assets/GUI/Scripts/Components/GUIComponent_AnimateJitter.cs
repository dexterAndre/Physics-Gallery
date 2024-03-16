using UnityEngine;

public class GUIComponent_AnimateJitter : GUIComponent
{
    [SerializeField] private GUIOption_IncrementalSlider controllerSpeed;
    [SerializeField] private GUIOption_Toggle controllerNormalize;
    [SerializeField] private GUIOption_Toggle controllerRelativeSpeed;

    public float Speed { get { return controllerSpeed.GetValue(); } }
    public bool Normalize { get { return controllerNormalize.GetValue(); } }
    public bool RelativeSpeed { get { return controllerRelativeSpeed.GetValue(); } }



    protected override void CheckReferences()
    {
        if (controllerSpeed == null ||
            controllerNormalize == null ||
            controllerRelativeSpeed == null)
        {
            Debug.LogWarning("GUI Options null. Cannot query GUI Component state.");
            return;
        }
    }

    public override void SetInteractable(bool state)
    {
        base.SetInteractable(state);
        controllerSpeed.SetInteractable(state);
        controllerNormalize.SetInteractable(state);
        controllerRelativeSpeed.SetInteractable(state);
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);
        // TODO: Implement
    }
}
