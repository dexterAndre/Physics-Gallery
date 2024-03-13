using UnityEngine;
using UnityEngine.UI;
using TMPro;

// TODO: CONTINUE: Component calls ApplyColor on GUIOption_Header, each header has a GUIOption_Header component

[ExecuteInEditMode]
public class GUIOption_Header : GUIOption2, IColorable
{
    [SerializeField] private Image background;
    [SerializeField] private GUIController_Toggle collapsibleToggle;
    [SerializeField] private Image dropdownGlyphOn;
    [SerializeField] private Image dropdownGlyphOff;
    [SerializeField] private Toggle enableToggle;
    [SerializeField] private Button buttonDelete;
    [SerializeField] private Button buttonMoveDown;
    [SerializeField] private Button buttonMoveUp;



    private void OnEnable()
    {
        FindReferences();
    }

    private void FindReferences()
    {
        Transform identifierGroup = transform.GetChild(0);
        if (identifierGroup != null)
        {
            if (background == null)
            {
                background = GetComponent<Image>();
            }
            if (collapsibleToggle == null)
            {
                collapsibleToggle = identifierGroup.GetChild(0).GetComponent<GUIController_Toggle>();
            }
            if (collapsibleToggle != null)
            {
                if (dropdownGlyphOn == null)
                {
                    dropdownGlyphOn = collapsibleToggle.transform.GetChild(0).GetComponent<Image>();
                }
                if (dropdownGlyphOff == null)
                {
                    dropdownGlyphOff = collapsibleToggle.transform.GetChild(1).GetComponent<Image>();
                }
            }
            if (enableToggle == null)
            {
                enableToggle = identifierGroup.GetChild(1).GetComponent<Toggle>();
            }
            if (descriptor == null)
            {
                descriptor = identifierGroup.GetChild(2).GetComponent<TMP_Text>();
            }
        }

        if (transform.childCount <= 1)
            return;

        Transform organizationGroup = transform.GetChild(1);
        if (organizationGroup != null)
        {
            if (buttonDelete == null)
            {
                buttonDelete = organizationGroup.GetChild(0).GetComponent<Button>();
            }
            if (buttonMoveDown == null)
            {
                buttonDelete = organizationGroup.GetChild(2).GetComponent<Button>();
            }
            if (buttonMoveUp == null)
            {
                buttonDelete = organizationGroup.GetChild(3).GetComponent<Button>();
            }
        }
    }

    public override void ApplyColorPalette(ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Header }
         * ---> { Identifier group }
         * -----> { Collapsible toggle }
         * -----> { Enable toggle }
         * -----> { Label }
         * ---> { Organization group }
         * -----> { Delete button }
         * -----> { Empty space }
         * -----> { Move down button }
         * -----> { Move up button }
         */

        base.ApplyColorPalette(palette);
        if (background != null)
        {
            IColorable.ApplyColorPalette_Image(background, palette.colorBackgroundPanel);
        }
        if (dropdownGlyphOn != null)
        {
            IColorable.ApplyColorPalette_Sprite(dropdownGlyphOn, palette.imageCollapsibleOn);
            IColorable.ApplyColorPalette_Image(dropdownGlyphOn, palette.colorFocusGlyph);
        }
        if (dropdownGlyphOff != null)
        {
            IColorable.ApplyColorPalette_Sprite(dropdownGlyphOff, palette.imageCollapsibleOff);
            IColorable.ApplyColorPalette_Image(dropdownGlyphOff, palette.colorFocusGlyph);
        }
        if (enableToggle != null)
        {
            IColorable.ApplyColorPalette_Toggle(enableToggle, palette);
        }
        if (descriptor != null)
        {
            IColorable.ApplyColorPalette_Label(descriptor, palette.colorFont);
        }
        if (buttonDelete != null)
        {
            IColorable.ApplyColorPalette_Button(buttonDelete, palette);
        }
        if (buttonMoveDown != null)
        {
            IColorable.ApplyColorPalette_Button(buttonMoveDown, palette);
        }
        if (buttonMoveUp != null)
        {
            IColorable.ApplyColorPalette_Button(buttonMoveUp, palette);
        }
    }

    public override void SetInteractable(bool state)
    {
        throw new System.NotImplementedException();
    }
}
