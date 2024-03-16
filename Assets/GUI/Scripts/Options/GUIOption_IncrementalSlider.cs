using UnityEngine;

public class GUIOption_IncrementalSlider : GUIOption
{
    [SerializeField] private GUIController_IncrementalSlider incrementalSlider;
    public GUIController_IncrementalSlider IncrementalSlider { get { return incrementalSlider; } }

    public float GetValue()
    {
        if (incrementalSlider == null)
        {
            Debug.LogWarning("GUIController_IncrementalSlider \"incrementalSlider\" is null. Cannot get value. Returning -1.");
            return -1f;
        }

        return incrementalSlider.SliderValue;
    }
    public override void SetInteractable(bool state)
    {
        incrementalSlider.ButtonDecrement.interactable = state;
        incrementalSlider.ButtonIncrement.interactable = state;
        incrementalSlider.Slider.interactable = state;
        incrementalSlider.InputField.interactable = state;
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        base.ApplyColorPalette(palette);
        IColorable.ApplyColorPalette_IncrementalSlider(IncrementalSlider, palette);
    }
}
