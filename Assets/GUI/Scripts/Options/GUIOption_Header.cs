using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GUIOption_Header : GUIOption2, IColorable
{
    [SerializeField] private Image background;
    [SerializeField] private Image dropdownGlyph;
    [SerializeField] private Toggle enableToggle;
    [SerializeField] private TMP_Text descriptor;
    [SerializeField] private Button buttonDelete;
    [SerializeField] private Button buttonMoveDown;
    [SerializeField] private Button buttonMoveUp;

    private void FindReferences()
    {
        if (background == null)
        {
            // find, then if null throw error
        }
    }

    public virtual void ApplyColorPalette(ColorPalette palette)
    {
        // 0. Background
        // 1. Collapsible caret
        // 2. Enable button
        // 3. Label
        // 4. Delete button
        // 5. Move down button
        // 6. Move up button

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

        // CONTINUE
        //dropdownGlyph.sprite = palette.dro
    }
}
