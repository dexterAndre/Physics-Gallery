using UnityEngine;



public class GUIComponent_Settings : GUIComponent, IPopulatable
{
    [SerializeField] private GUIOption_ToggleFlip controllerDimension;
    [SerializeField] private GUIOption_Toggle controllerShowBounds;
    [SerializeField] private GUIOption_Dropdown controllerBounds;
    [SerializeField] private GUIOption_Dropdown controllerEdgeResponse;

    public bool Is2D { get { return controllerDimension.GetValue(); } }
    public bool ShowBounds { get { return controllerShowBounds.GetValue(); } }
    public int Bounds { get { return controllerBounds.GetValue(); } }
    public int EdgeResponse { get { return controllerEdgeResponse.GetValue(); } }



    protected override void CheckReferences()
    {
        if (controllerDimension == null ||
            controllerShowBounds == null ||
            controllerBounds == null ||
            controllerEdgeResponse == null)
        {
            Debug.LogWarning("GUI Options null. Cannot query GUI Component state.");
            return;
        }
    }

    public override void SetInteractable(bool state)
    {
        base.SetInteractable(state);
        controllerDimension.SetInteractable(state);
        controllerShowBounds.SetInteractable(state);
        controllerBounds.SetInteractable(state);
        controllerEdgeResponse.SetInteractable(state);
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);
        controllerDimension.ApplyColorPalette(palette);
        controllerShowBounds.ApplyColorPalette(palette);
        controllerBounds.ApplyColorPalette(palette);
        controllerEdgeResponse.ApplyColorPalette(palette);
    }

    public void Populate()
    {
        IPopulatable.Populate_Dropdown<BoundsType>(controllerBounds.Dropdown, Manager_Lookup.Instance.ManagerGUI.NameList_Bounds);
        IPopulatable.Populate_Dropdown<EdgeResponse>(controllerEdgeResponse.Dropdown, Manager_Lookup.Instance.ManagerGUI.NameList_EdgeResponse);
    }
}
