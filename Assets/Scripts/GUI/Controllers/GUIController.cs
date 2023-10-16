using Shapes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/*
    To do:
    - Apply colors on all referenced objects properly
    - Measure height and apply to aspect ratio fitter (also apply to height)
*/

public abstract class GUIController : MonoBehaviour
{
    [SerializeField] protected Transform componentParent;
    [SerializeField] protected GUIManager guiManager;

    public abstract void ApplyColorPalette(ColorPalette palette);

    public static void SetHeaderColors(Image header, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Image }
         * ---> { }                     // Identifier group
         * -----> { Button, image }     // Collapse button
         * -----> { Label }
         * ---> { }                     // Organization buttons
         * -----> { Button }            // Delete button
         * -----> { }
         * -----> { }
         * -----> { }
         * -----> { Button }            // Down button
         * -----> { }
         * -----> { Button }            // Up button
         */

        Button collapseButton = header.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        TMP_Text identifierLabel = header.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>();
        Transform organizationGroup = header.transform.GetChild(1);
        Button deleteButton = organizationGroup.GetChild(0).GetComponent<Button>();
        Button downButton = organizationGroup.GetChild(4).GetComponent<Button>();
        Button upButton = organizationGroup.GetChild(6).GetComponent<Button>();

        header.color = palette.colorForegroundFill;
        EditorUtility.SetDirty(header);
        SetButtonColors(collapseButton, palette);
        identifierLabel.color = palette.colorFont;
        EditorUtility.SetDirty(identifierLabel);
        SetButtonColors(deleteButton, palette);
        SetButtonColors(downButton, palette);
        SetButtonColors(upButton, palette);
    }
    public static void SetButtonColors(Button button, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Button, stroke image }
         * ---> { Fill image }
         * -----> { Icon }
         */

        Image strokeImage = button.GetComponent<Image>();
        Image fillImage = button.transform.GetChild(0).GetComponent<Image>();
        Image glyphImage = fillImage.transform.GetChild(0).GetComponent<Image>();
        TMP_Text buttonLabel = fillImage.transform.GetChild(0).GetComponent<TMP_Text>();

        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = palette.colorClickableNormal;
        colorBlock.highlightedColor = palette.colorClickableHighlighted;
        colorBlock.pressedColor = palette.colorClickablePressed;
        colorBlock.selectedColor = palette.colorClickableSelected;
        colorBlock.disabledColor = palette.colorClickableDisabled;
        button.colors = colorBlock;
        EditorUtility.SetDirty(button);

        strokeImage.color = palette.colorFocusStroke;
        EditorUtility.SetDirty(strokeImage);
        fillImage.color = palette.colorFocusFill;
        EditorUtility.SetDirty(fillImage);
        if (glyphImage != null)
        {
            glyphImage.color = palette.colorFocusGlyph;
            EditorUtility.SetDirty(glyphImage);
        }
        else if (buttonLabel != null)
        {
            buttonLabel.color = palette.colorFont;
        }
    }
    public static void SetToggleColors(Toggle toggle, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Toggle, stroke image }
         * ---> { Fill image }
         * -----> { Glyph image }
         */

        Image strokeImage = toggle.GetComponent<Image>();
        Image fillImage = toggle.transform.GetChild(0).GetComponent<Image>();
        Image glyphImage = fillImage.transform.GetChild(0).GetComponent<Image>();

        ColorBlock colorBlock = toggle.colors;
        colorBlock.normalColor = palette.colorClickableNormal;
        colorBlock.highlightedColor = palette.colorClickableHighlighted;
        colorBlock.pressedColor = palette.colorClickablePressed;
        colorBlock.selectedColor = palette.colorClickableSelected;
        colorBlock.disabledColor = palette.colorClickableDisabled;
        toggle.colors = colorBlock;
        EditorUtility.SetDirty(toggle);

        strokeImage.color = palette.colorFocusStroke;
        EditorUtility.SetDirty(strokeImage);
        fillImage.color = palette.colorFocusFill;
        EditorUtility.SetDirty(fillImage);
        glyphImage.color = palette.colorFocusGlyph;
        EditorUtility.SetDirty(glyphImage);
    }
    public static void SetFlipToggleColors(Button button, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Button, slot image }
         * ---> { Left label }
         * ---> { Stroke image }
         * -----> { Fill image }
         * ---> { Right label }
         */

        Image slotImage = button.GetComponent<Image>();
        TMP_Text leftLabel = button.transform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text rightLabel = button.transform.GetChild(2).GetComponent<TMP_Text>();
        Image strokeImage = button.transform.GetChild(1).GetComponent<Image>();
        Image fillImage = strokeImage.transform.GetChild(0).GetComponent<Image>();

        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = palette.colorClickableNormal;
        colorBlock.highlightedColor = palette.colorClickableHighlighted;
        colorBlock.pressedColor = palette.colorClickablePressed;
        colorBlock.selectedColor = palette.colorClickableSelected;
        colorBlock.disabledColor = palette.colorClickableDisabled;
        button.colors = colorBlock;
        EditorUtility.SetDirty(button);

        slotImage.color = palette.colorForegroundFill;
        EditorUtility.SetDirty(slotImage);
        leftLabel.color = palette.colorFont;
        EditorUtility.SetDirty(leftLabel);
        strokeImage.color = palette.colorFocusStroke;
        EditorUtility.SetDirty(strokeImage);
        fillImage.color = palette.colorFocusFill;
        EditorUtility.SetDirty(fillImage);
        rightLabel.color = palette.colorFont;
        EditorUtility.SetDirty(rightLabel);
    }
    public static void SetDropdownColors(TMP_Dropdown dropdown, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Dropdown, slot image }
         * ---> { }
         * -----> { Button stroke image }
         * -------> { Button fill image }
         * ---------> { Button glyph image }
         * ---> { Label }
         */

        Image slotImage = dropdown.GetComponent<Image>();
        Image buttonStrokeImage = dropdown.transform.GetChild(0).GetComponent<Image>();
        Image buttonFillImage = buttonStrokeImage.transform.GetChild(0).GetComponent<Image>();
        Image buttonGlyphImage = buttonFillImage.transform.GetChild(0).GetComponent<Image>();
        TMP_Text label = dropdown.transform.GetChild(1).GetComponent<TMP_Text>();

        ColorBlock colorBlock = dropdown.colors;
        colorBlock.normalColor = palette.colorClickableNormal;
        colorBlock.highlightedColor = palette.colorClickableHighlighted;
        colorBlock.pressedColor = palette.colorClickablePressed;
        colorBlock.selectedColor = palette.colorClickableSelected;
        colorBlock.disabledColor = palette.colorClickableDisabled;
        dropdown.colors = colorBlock;
        EditorUtility.SetDirty(dropdown);

        slotImage.color = palette.colorForegroundFill;
        EditorUtility.SetDirty(slotImage);
        buttonStrokeImage.color = palette.colorFocusStroke;
        EditorUtility.SetDirty(buttonStrokeImage);
        buttonFillImage.color = palette.colorFocusFill;
        EditorUtility.SetDirty(buttonFillImage);
        buttonGlyphImage.color = palette.colorFocusGlyph;
        EditorUtility.SetDirty(buttonGlyphImage);
        label.color = palette.colorFont;
        EditorUtility.SetDirty(label);
    }
    public static void SetSliderInputColors(GUIIncrementSliderInput slider, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { }                                               // Script
         * ---> { Left button, left button stroke }
         * -----> { Left button fill }
         * -------> { Left button glyph }
         * ---> { }                                             // Slider group
         * -----> { }                                           // Slider
         * -------> { Slider slot image }
         * -------> { Slider fill image }
         * ---> { Slider input text (highlighted) }             // Do not change
         * -----> { }                                           // Mask area
         * -------> { Slider text }
         * ---> { Right button, right button stroke }
         * -----> { Right button fill }
         * -------> { Right button glyph }
         */

        Button leftButton = slider.transform.GetChild(0).GetComponent<Button>();
        Transform sliderGroup = slider.transform.GetChild(1);
        Image sliderBackground = sliderGroup.GetChild(0).GetChild(0).GetComponent<Image>();
        Image sliderFill = sliderGroup.GetChild(0).GetChild(1).GetComponent<Image>();
        TMP_Text inputText = sliderGroup.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        Button rightButton = slider.transform.GetChild(2).GetComponent<Button>();

        SetButtonColors(leftButton, palette);
        sliderBackground.color = palette.colorBackgroundFill;
        EditorUtility.SetDirty(sliderBackground);
        sliderFill.color = palette.colorForegroundFill;
        EditorUtility.SetDirty(sliderFill);
        inputText.color = palette.colorFont;
        EditorUtility.SetDirty(inputText);
        SetButtonColors(rightButton, palette);
    }
    public static void SetRadioButtonColors(Toggle toggle, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Toggle }
         * ---> { Stroke image }
         * -----> { Fill image }
         * -------> { Glyph image }
         */

        Image strokeImage = toggle.GetComponent<Image>();
        Image fillImage = strokeImage.transform.GetChild(0).GetComponent<Image>();
        Image glyphImage = fillImage.transform.GetChild(0).GetComponent<Image>();

        ColorBlock colorBlock = toggle.colors;
        colorBlock.normalColor = palette.colorClickableNormal;
        colorBlock.highlightedColor = palette.colorClickableHighlighted;
        colorBlock.pressedColor = palette.colorClickablePressed;
        colorBlock.selectedColor = palette.colorClickableSelected;
        colorBlock.disabledColor = palette.colorClickableDisabled;
        toggle.colors = colorBlock;
        EditorUtility.SetDirty(toggle);

        strokeImage.color = palette.colorForegroundFill;
        EditorUtility.SetDirty(strokeImage);
        fillImage.color = palette.colorBackgroundFill;
        EditorUtility.SetDirty(fillImage);
        glyphImage.color = palette.colorFocusGlyph;
        EditorUtility.SetDirty(glyphImage);
    }
}
