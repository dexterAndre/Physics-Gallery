using UnityEngine;

public class GUIComponent_AnimateStrangeAttractor2 : GUIComponent2
{
    [SerializeField] private GUIOption_IncrementalSlider2 controllerSpeed;
    [SerializeField] private GUIOption_Dropdown2 controllerType;

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
}
