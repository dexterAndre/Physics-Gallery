using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;



public class GUIConditionalEnable : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    public Toggle Toggle { get { return toggle; } }
    [SerializeField] private List<CanvasGroup> canvases;
    [SerializeField] private float inactiveAlpha = 0.5f;
    [SerializeField] private bool isInToggleGroup = false;
    [SerializeField] bool staticToggle = true;



    private void Awake()
    {
        if (canvases.Count <= 0)
        {
            Debug.LogError("Error: GUIConditionalEnable requires CanvasGroup components inside the selectablesParents list to function.");
            gameObject.SetActive(false);
        }

        OnToggled();
    }

    private void OnEnable()
    {
        toggle?.onValueChanged.AddListener(SetEnabled);
    }

    private void OnDisable()
    {
        toggle?.onValueChanged.RemoveListener(SetEnabled);
    }

    public void OnToggled()
    {
        SetEnabled(InterpretStatus());
    }

    private void SetEnabled(bool state)
    {
        if (isInToggleGroup)
        {
            foreach (CanvasGroup canvasGroup in canvases)
            {
                canvasGroup.interactable = state;
                canvasGroup.alpha = state ? 1f : inactiveAlpha;
            }
        }
        else
        {

            foreach (CanvasGroup canvasGroup in canvases)
            {
                canvasGroup.interactable = state;
                canvasGroup.alpha = state ? 1f : inactiveAlpha;

                GUIConditionalEnable foundComponent = canvasGroup.GetComponent<GUIConditionalEnable>();
                if (foundComponent != null && foundComponent.Toggle != null)
                {
                    // Caller's state supersedes substate
                    bool subState = state && foundComponent.Toggle.isOn;

                    canvasGroup.interactable = subState;
                    canvasGroup.alpha = subState ? 1f : inactiveAlpha;
                    // Setting Toggles to interactable if whole group is activated, non-interactable if whole group is deactivated
                    foundComponent.Toggle.GetComponent<CanvasGroup>().interactable = state;
                }
            }
        }
    }

    private bool InterpretStatus()
    {
        return toggle != null ? toggle.isOn : staticToggle;
    }
}
