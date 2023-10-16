using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController_Header : GUIController
{
    private Button buttonUp;
    private Button buttonDown;
    private Toggle enableToggle;
    public bool IsEnabled
    {
        get
        {
            ConditionalSetButtons();
            return enableToggle.isOn;
        }
    }



    public override void ApplyColorPalette(ColorPalette palette)
    {
        SetHeaderColors(GetComponent<Image>(), palette);
    }

    private void ConditionalSetComponentParent()
    {
        // All headers should be under the component GameObject, which should be under the component parent GameObject
        if (componentParent == null)
        {
            componentParent = transform.parent;
            if (componentParent == null)
            {
                Debug.LogWarning("Could not find component parent.");
                return;
            }
        }
    }

    private void ConditionalSetGUIManager()
    {
        if (guiManager == null)
        {
            guiManager = GameObject.Find("pnl_menu").GetComponent<GUIManager>();
            if (guiManager == null)
            {
                Debug.LogWarning("Could not find GUIManager.");
                return;
            }
        }
    }

    private void ConditionalSetButtons()
    {
        if (buttonUp == null || buttonDown == null || enableToggle == null)
        {
            Transform organizationButtons = transform.Find("grp_organization-buttons");
            if (organizationButtons == null)
            {
                Debug.LogWarning("Could not find grp_organization-buttons.");
                return;
            }
            Transform identifierBar = transform.Find("grp_identifier");
            if (identifierBar == null)
            {
                Debug.LogWarning("Could not find grp_identifier.");
                return;
            }
            Transform checkbox = identifierBar.Find("grp_toggle-checkbox");
            if (checkbox == null)
            {
                Debug.LogWarning("Could not find grp_toggle-checkbox.");
                return;
            }

            buttonUp = organizationButtons.Find("btn_move-up").GetComponent<Button>();
            buttonDown = organizationButtons.Find("btn_move-down").GetComponent<Button>();
            enableToggle = checkbox.GetChild(0).GetComponent<Toggle>();
        }
    }

    public void RemoveComponent()
    {
        ConditionalSetComponentParent();

        GUIComponent component = componentParent.GetComponent<GUIComponent>();
        if (component == null)
        {
            Debug.LogWarning("Could not find header's GUIComponent parent.");
            return;
        }

        // 1. Remove script from behavior parent
        GameObject behaviorObject = component.Behavior.gameObject;
        ManagerPointSet pointSetManager = behaviorObject.transform.parent.GetComponent<ManagerPointSet>();
        if (pointSetManager == null)
        {
            Debug.LogWarning("Could not find ManagerPointSet.");
            return;
        }
        pointSetManager.RemoveBehavior(component.Behavior);

        // 2. Delete behavior object
        Destroy(behaviorObject.gameObject);

        // 3. Delete GUI object
        Destroy(componentParent.gameObject);

        // 4. Check if neighbors need to disable their reorganizational buttons
        // TODO
    }

    public void MoveComponent(bool moveUp)
    {
        ConditionalSetComponentParent();
        ConditionalSetGUIManager();

        if (moveUp && !CanMoveUp())
            return;

        if (!moveUp && !CanMoveDown())
            return;

        // Moves component in GUI hierarchy
        int oldSiblingIndex = componentParent.GetSiblingIndex();
        int indexDirection = moveUp ? -1 : 1;
        componentParent.SetSiblingIndex(oldSiblingIndex + indexDirection);
        LayoutRebuilder.ForceRebuildLayoutImmediate(componentParent.parent.GetComponent<RectTransform>());

        // Updates button disability
        SetButtonDisability();

        // If 2 or more reorganizable components, re-evaluate swapped component's disability
        if (componentParent.parent.childCount >= guiManager.LockedComponents + 2)
        {
            // Finds swapped header
            componentParent.parent.GetChild(oldSiblingIndex).GetChild(0).GetComponent<GUIController_Header>().SetButtonDisability();
        }
    }

    public void SetButtonDisability()
    {
        ConditionalSetButtons();

        buttonUp.interactable = CanMoveUp();
        buttonDown.interactable = CanMoveDown();
    }

    private bool CanMoveUp()
    {
        ConditionalSetGUIManager();

        // Cannot move above locked components at the top of the menu
        return componentParent.GetSiblingIndex() > guiManager.LockedComponents;
    }

    private bool CanMoveDown()
    {
        ConditionalSetGUIManager();

        // Cannot move below the "Add Component" button at the bottom of the menu
        return componentParent.GetSiblingIndex() < componentParent.parent.childCount - 2;
    }
}
