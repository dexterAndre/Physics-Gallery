using System.Collections;
using UnityEngine;
using UnityEngine.UI;



// TODO: Convert into GUIOption instead of GUIController
// TODO: Disable corresponding organizational buttons if they are at top or bottom
// TODO: After repositioning, disable corresponding organizational buttons if they are at top or bottom
public class GUIController_Organization : MonoBehaviour
{
    private PointBehavior pointBehavior;
    [SerializeField] private Button buttonDelete;
    [SerializeField] private Button buttonMoveDown;
    [SerializeField] private Button buttonMoveUp;

    private void OnEnable()
    {
        if (Manager_Lookup.Instance.ManagerGUI == null)
        {
            Debug.LogWarning("Cannot find Manager_GUI for GUIController_Organization. Disabling organization bar's functionality.");
            gameObject.SetActive(false);
        }

        if (Manager_Lookup.Instance.ManagerPointSet == null)
        {
            Debug.LogWarning("Cannot find Manager_PointSet for GUIController_Organization. Disabling organization bar's functionality.");
            gameObject.SetActive(false);
        }

        if (pointBehavior == null)
        {
            pointBehavior = transform.parent.parent.GetComponent<PointBehavior>();
            if (pointBehavior == null)
            {
                Debug.LogWarning("Could not find PointBehavior for this GUIController_Organization. Disabling organization bar's functionality.");
                gameObject.SetActive(false);
            }
        }

        /**
         * Assuming following hierarchy:
         * -> Organization bar { GUIController_Organization }
         * ---> buttonDelete { Button } 
         * ---> space { }
         * ---> buttonMoveDown { Button }
         * ---> buttonMoveUp { Button }
         */

        if (buttonDelete == null)
        {
            buttonDelete = transform.GetChild(0).GetComponent<Button>();
        }
        if (buttonDelete == null)
        {
            Debug.LogWarning("Cannot find buttonDelete for GUIController_Organization. Deleting component will not work.");
        }
        else
        {
            buttonDelete.onClick.RemoveAllListeners();
            buttonDelete.onClick.AddListener(DeleteComponent);
        }

        if (buttonMoveDown == null)
        {
            buttonMoveDown = transform.GetChild(2).GetComponent<Button>();
        }
        if (buttonMoveDown == null)
        {
            Debug.LogWarning("Cannot find buttonDelete for GUIController_Organization. Moving component down will not work.");
        }
        else
        {
            buttonMoveDown.onClick.RemoveAllListeners();
            buttonMoveDown.onClick.AddListener(MoveComponentDown);
        }

        if (buttonMoveUp == null)
        {
            buttonMoveUp = transform.GetChild(3).GetComponent<Button>();
        }
        if (buttonMoveUp == null)
        {
            Debug.LogWarning("Cannot find buttonDelete for GUIController_Organization. Moving component up will not work.");
        }
        else
        {
            buttonMoveUp.onClick.RemoveAllListeners();
            buttonMoveUp.onClick.AddListener(MoveComponentUp);
        }

        // Conditionally enabling/disabling repositioning buttons
        buttonMoveUp.interactable = !IsTopComponent();
        buttonMoveDown.interactable = !IsBottomComponent();
    }

    public void DeleteComponent()
    {
        // TODO: Differentiate between animation, overlay, and selection
        Manager_Lookup.Instance.ManagerPointSet.RemoveBehavior(pointBehavior);
        Destroy(pointBehavior.gameObject);
        // TODO: May count destroy-queued object - delay
        Manager_Lookup.Instance.ManagerGUI.ConditionalSetRepositioningButtonsInteractive();
        LayoutRebuilder.ForceRebuildLayoutImmediate(pointBehavior.transform.parent.GetComponent<RectTransform>());
    }

    public void RepositionComponent(bool InShouldMoveUp)
    {
        if (buttonDelete == null)
        {
            Debug.LogWarning("Attempted to delete component, but buttonDelete is null. Aborting deletion operation.");
            return;
        }

        Transform componentTransform = transform.parent.parent;
        PointBehavior pointBehavior = componentTransform.GetComponent<PointBehavior>();
        if (pointBehavior == null)
        {
            Debug.LogWarning("Could not find PointBehavior for this GUIController_Organization. Aborting deletion operation.");
            return;
        }

        if (componentTransform.parent.childCount != Manager_Lookup.Instance.ManagerPointSet.AnimationBehaviors.Count)
        {
            Debug.LogWarning("Manager_GUI's component count not in sync with Manager_PointSet's behaviors count. Aborting repositioning operation.");
            return;
        }

        int siblingIndex = componentTransform.GetSiblingIndex();
        if (pointBehavior != Manager_Lookup.Instance.ManagerPointSet.AnimationBehaviors[siblingIndex])
        {
            Debug.LogWarning("This component's PointBehavior is not equal to Manager_PointSet's behavior at the same child/list index. Aborting repositioning operation.");
            return;
        }

        if (InShouldMoveUp == true)
        {
            if (IsTopComponent())
            {
                Debug.LogWarning("Attempting to move component up, but it is already at the top. Aborting repositioning operation.");
                return;
            }

            // PointBehavior list
            Manager_Lookup.Instance.ManagerPointSet.AnimationBehaviors[siblingIndex] = Manager_Lookup.Instance.ManagerPointSet.AnimationBehaviors[siblingIndex - 1];
            Manager_Lookup.Instance.ManagerPointSet.AnimationBehaviors[siblingIndex - 1] = pointBehavior;

            // GUI position
            componentTransform.SetSiblingIndex(siblingIndex - 1);
        }
        else
        {
            if (IsBottomComponent())
            {
                Debug.LogWarning("Attempting to move component down, but it is already at the bottom. Aborting repositioning operation.");
                return;
            }

            // PointBehavior list
            Manager_Lookup.Instance.ManagerPointSet.AnimationBehaviors[siblingIndex] = Manager_Lookup.Instance.ManagerPointSet.AnimationBehaviors[siblingIndex + 1];
            Manager_Lookup.Instance.ManagerPointSet.AnimationBehaviors[siblingIndex + 1] = pointBehavior;

            // GUI position
            componentTransform.SetSiblingIndex(siblingIndex + 1);
        }

        StartCoroutine(ConditionalSetRepositioningButtonsInteractive_NextFrame());
    }

    public void MoveComponentDown()
    {
        RepositionComponent(false);
    }

    public void MoveComponentUp()
    {
        RepositionComponent(true);
    }

    private bool IsTopComponent()
    {
        return pointBehavior.transform.GetSiblingIndex() == 0;
    }

    private bool IsBottomComponent()
    {
        return pointBehavior.transform.GetSiblingIndex() == (pointBehavior.transform.parent.childCount - 1);
    }

    public void ConditionalSetButtonInteractive()
    {
        buttonMoveUp.interactable = !IsTopComponent();
        buttonMoveDown.interactable = !IsBottomComponent();
    }

    private IEnumerator ConditionalSetRepositioningButtonsInteractive_NextFrame()
    {
        yield return new WaitForEndOfFrame();
        Manager_Lookup.Instance.ManagerGUI.ConditionalSetRepositioningButtonsInteractive();
        LayoutRebuilder.ForceRebuildLayoutImmediate(pointBehavior.transform.parent.GetComponent<RectTransform>());
        yield return null;
    }
}
