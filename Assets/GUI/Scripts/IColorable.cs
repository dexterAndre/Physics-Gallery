using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;



public interface IColorable
{
    public abstract void ApplyColorPalette(ColorPalette palette);

    protected static void ApplyColorPalette_Label(TMP_Text label, Color color)
    {
        label.color = color;
        EditorUtility.SetDirty(label);
    }
    protected static void ApplyColorPalette_Image(Image image, Color color)
    {
        image.color = color;
        EditorUtility.SetDirty(image);
    }
    protected static void ApplyColorPalette_Sprite(Image image, Sprite sprite)
    {
        image.sprite = sprite;
        EditorUtility.SetDirty(image);
    }
    protected static void ApplyColorPalette_ColorBlock(Selectable target, ColorPalette palette)
    {
        ColorBlock colorBlock = target.colors;
        colorBlock.normalColor = palette.colorClickableNormal;
        colorBlock.highlightedColor = palette.colorClickableHighlighted;
        colorBlock.pressedColor = palette.colorClickablePressed;
        colorBlock.selectedColor = palette.colorClickableSelected;
        colorBlock.disabledColor = palette.colorClickableDisabled;
        target.colors = colorBlock;
        EditorUtility.SetDirty(target);
    }
    protected static void ApplyColorPalette_StrokeFillGlyphLabel(ColorPalette palette, Image strokeImage, Image fillImage, Image glyphImage = null, TMP_Text label = null)
    {
        ApplyColorPalette_Image(strokeImage, palette.colorFocusStroke);
        ApplyColorPalette_Image(fillImage, Color.white);
        if (glyphImage != null)
        {
            ApplyColorPalette_Image(glyphImage, palette.colorFocusGlyph);
        }
        if (label  != null)
        {
            ApplyColorPalette_Label(label, palette.colorFont);
        }
    }
    protected static void ApplyColorPalette_Button(Button button, ColorPalette palette, Sprite glyph = null)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Button, stroke image }
         * ---> { Fill image }
         * -----> { Icon / label }
         */

        Image strokeImage = button.GetComponent<Image>();
        Image fillImage = button.transform.GetChild(0).GetComponent<Image>();
        Image glyphImage = fillImage.transform.GetChild(0).GetComponent<Image>();
        TMP_Text buttonLabel = fillImage.transform.GetChild(0).GetComponent<TMP_Text>();

        ApplyColorPalette_ColorBlock(button, palette);
        ApplyColorPalette_Image(strokeImage, palette.colorFocusStroke);
        ApplyColorPalette_Image(fillImage, Color.white);
        if (glyphImage != null)
        {
            ApplyColorPalette_Image(glyphImage, palette.colorFocusGlyph);
            ApplyColorPalette_Sprite(glyphImage, glyph);
        }
        else if (buttonLabel != null)
        {
            ApplyColorPalette_Label(buttonLabel, palette.colorFont);
        }
    }
    protected static void ApplyColorPalette_Toggle(Toggle toggle, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Toggle, stroke image }
         * ---> { Fill image }
         * -----> { Glyph image }
         */

        Image strokeImage = toggle.GetComponent<Image>();
        Image fillImage = strokeImage.transform.GetChild(0).GetComponent<Image>();
        Image glyphImage = fillImage.transform.GetChild(0).GetComponent<Image>();

        ApplyColorPalette_ColorBlock(toggle, palette);
        ApplyColorPalette_Image(strokeImage, palette.colorFocusStroke);
        ApplyColorPalette_Image(fillImage, Color.white);
        if (glyphImage != null)
        {
            ApplyColorPalette_Image(glyphImage, palette.colorFocusGlyph);
            if (palette.imageCheckmark != null)
            {
                ApplyColorPalette_Sprite(glyphImage, palette.imageCheckmark);
            }
        }
    }
    protected static void ApplyColorPalette_FlipToggle(Toggle flipToggle, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Toggle }
         * ---> { Label left }
         * ---> { Slot fill image }
         * -----> { Button left stroke image }
         * -------> { Button left fill image }
         * -----> { Button right stroke image }
         * -------> { Button right fill image }
         * ---> { Label right }
         */

        TMP_Text labelLeft = flipToggle.transform.GetChild(0).GetComponent<TMP_Text>();
        Image slotFillImage = flipToggle.transform.GetChild(1).GetComponent<Image>();
        Image buttonLeftStrokeImage = slotFillImage.transform.GetChild(0).GetComponent<Image>();
        Image buttonLeftFillImage = buttonLeftStrokeImage.transform.GetChild(0).GetComponent<Image>();
        Image buttonRightStrokeImage = slotFillImage.transform.GetChild(1).GetComponent<Image>();
        Image buttonRightFillImage = buttonRightStrokeImage.transform.GetChild(0).GetComponent<Image>();
        TMP_Text labelRight = flipToggle.transform.GetChild(2).GetComponent<TMP_Text>();

        ApplyColorPalette_ColorBlock(flipToggle, palette);
        ApplyColorPalette_Label(labelLeft, palette.colorFont);
        ApplyColorPalette_Image(slotFillImage, palette.colorForegroundFill);
        ApplyColorPalette_Image(buttonLeftStrokeImage, palette.colorFocusStroke);
        ApplyColorPalette_Image(buttonLeftFillImage, Color.white);
        ApplyColorPalette_Image(buttonRightStrokeImage, palette.colorFocusStroke);
        ApplyColorPalette_Image(buttonRightFillImage, Color.white);
        ApplyColorPalette_Label(labelRight, palette.colorFont);
    }
    protected static void ApplyColorPalette_Dropdown(TMP_Dropdown dropdown, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Dropdown, slot image }
         * ---> { Label }
         * ---> { Template, menu background image }
         * -----> { Viewport }
         * -------> { Content }
         * ---------> { Item, Toggle }
         * -----------> { Item Background, item background fill }
         * -----------> { Item Checkmark, item glyph }
         * -----------> { Item Label, item label }
         */

        Image dropdownSlotFill = dropdown.GetComponent<Image>();
        TMP_Text labelDropdown = dropdownSlotFill.transform.GetChild(0).GetComponent<TMP_Text>();
        Image templateBackgroundFill = dropdownSlotFill.transform.GetChild(1).GetComponent<Image>();
        Toggle itemToggle = templateBackgroundFill.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Toggle>();
        Image itemBackgroundFill = itemToggle.transform.GetChild(0).GetComponent<Image>();
        Image itemCheckmarkGlyph = itemToggle.transform.GetChild(1).GetComponent<Image>();
        TMP_Text itemLabel = itemToggle.transform.GetChild(2).GetComponent<TMP_Text>();

        ApplyColorPalette_ColorBlock(dropdown, palette);
        ApplyColorPalette_Image(dropdownSlotFill, palette.colorForegroundFill);
        ApplyColorPalette_Label(labelDropdown, palette.colorFont);
        ApplyColorPalette_Image(templateBackgroundFill, palette.colorForegroundFill);
        ApplyColorPalette_ColorBlock(itemToggle, palette);
        ApplyColorPalette_Image(itemBackgroundFill, Color.white);
        ApplyColorPalette_Image(itemCheckmarkGlyph, palette.colorFocusGlyph);
        ApplyColorPalette_Label(itemLabel, palette.colorFont);
    }
    protected static void ApplyColorPalette_DropdownGallery(GUIController_DropdownGallery dropdownGallery, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Dropdown gallery }
         * ---> { Button decrement }
         * ---> { Dropdown }
         * ---> { Button increment }
         */

        Button buttonDecrement = dropdownGallery.transform.GetChild(0).GetComponent<Button>();
        TMP_Dropdown dropdown = dropdownGallery.transform.GetChild(1).GetComponent<TMP_Dropdown>();
        Button buttonIncrement = dropdownGallery.transform.GetChild(2).GetComponent<Button>();

        ApplyColorPalette_Button(buttonDecrement, palette, palette.imageDecrement);
        ApplyColorPalette_Dropdown(dropdown, palette);
        ApplyColorPalette_Button(buttonIncrement, palette, palette.imageIncrement);
    }
    protected static void ApplyColorPalette_IncrementalSlider(GUIController_IncrementalSlider incrementalSlider, ColorPalette palette)
    {
        /**
         * Assuming following hierarchical layout:
         * -> { Incremental slider }
         * ---> { Button decrement }
         * ---> { Slider group }
         * -----> { Slider slot image }
         * -----> { Slider fill image }
         * ---> { Slider input text (highlighted) }             // Do not change
         * -----> { }                                           // Mask area
         * -------> { Slider text }
         * ---> { Button increment }
         */

        Button buttonDecrement = incrementalSlider.transform.GetChild(0).GetComponent<Button>();
        Transform sliderGroup = incrementalSlider.transform.GetChild(1);
        Image sliderBackground = sliderGroup.GetChild(0).GetChild(0).GetComponent<Image>();
        Image sliderFill = sliderGroup.GetChild(0).GetChild(1).GetComponent<Image>();
        TMP_Text inputText = sliderGroup.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        Button buttonIncrement = incrementalSlider.transform.GetChild(2).GetComponent<Button>();

        ApplyColorPalette_Button(buttonDecrement, palette, palette.imageDecrement);
        ApplyColorPalette_Image(sliderBackground, palette.colorForegroundFill);
        ApplyColorPalette_Image(sliderFill, palette.colorBackgroundPanel);
        ApplyColorPalette_Label(inputText, palette.colorFont);
        ApplyColorPalette_Button(buttonIncrement, palette, palette.imageIncrement);
    }
}
