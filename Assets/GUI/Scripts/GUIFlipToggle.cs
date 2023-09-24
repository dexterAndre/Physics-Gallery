using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIFlipToggle : MonoBehaviour
{
    // Fucntionality
    private bool isFlippedLeft = true;
    [SerializeField] private float flipDuration = 0.25f;
    private float flipTimer = 0.25f;
    [SerializeField] private AnimationCurve flipCurve;

    // References
    [SerializeField] private Transform parentPanel;
    private Image toggleButton;
    private Image toggleSlotBackground;
    private bool scheduledInitialPositioning = false;   // Hack to delay some Awake() functionality until after GUI values have been read (since they are not available on Awake())



    private void Awake()
    {
        if (parentPanel == null)
        {
            Transform currentParent = transform.parent;
            while (parentPanel == null && currentParent != null)
            {
                if (currentParent.GetComponent<GUIResizeEventListener>() != null)
                {
                    parentPanel = currentParent;
                }
                else
                {
                    currentParent = currentParent.parent;
                }
            }

            if (parentPanel == null)
            {
                Debug.LogWarning("Warning: parentPanel is not set. Will not respond to resize events.");
            }
        }

        if (flipDuration < 0.0f)
        {
            Debug.LogWarning("Warning: Flip animation duration is less than zero. Forcing it to be 0, which means an instant animation.");
        }

        if (toggleButton == null)
        {
            toggleButton = transform.GetChild(1).GetComponent<Image>();
            if (toggleButton == null)
            {
                Debug.LogError("Error: Could not find button for Flip Toggle. The first child of a Flip Toggle should always contain a Button component. Have you modified or applied a Flip Toggle component to any objects whose first child is not a button?");
                gameObject.SetActive(false);
            }
        }

        if (toggleSlotBackground == null)
        {
            toggleSlotBackground = GetComponent<Image>();
            if (toggleSlotBackground == null)
            {
                Debug.LogError("Error: Could not find Flip Toggle's background image component. It needs this to calculate where the button moves to, even if it isn't supposed to be visible.");
                gameObject.SetActive(false);
            }
        }

        flipTimer = flipDuration;
        scheduledInitialPositioning = true;
    }

    private void OnEnable()
    {
        if (parentPanel != null)
        {
            parentPanel.GetComponent<GUIResizeEventListener>().onResizeEvent += UpdateButtonPosition;
        }
    }

    private void OnDisable()
    {
        if (parentPanel != null)
        {
            parentPanel.GetComponent<GUIResizeEventListener>().onResizeEvent -= UpdateButtonPosition;
        }
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void OnGUI()
    {
        if (scheduledInitialPositioning)
        {
            Vector3 leftPosition = new Vector3(CalculateCoordinate(true), 0f, 0f);
            SetButtonPosition(leftPosition);
            scheduledInitialPositioning = false;
        }
    }

    private void UpdateTimer()
    {
        if (flipDuration > 0.0f && flipTimer < flipDuration)
        {
            flipTimer += Time.deltaTime;
            if (flipTimer > flipDuration)
            {
                flipTimer = flipDuration;
            }

            SetButtonPosition();
        }
    }

    public bool IsFlippedLeft()
    {
        return isFlippedLeft;
    }

    public bool IsFlippedRight()
    {
        return !IsFlippedLeft();
    }

    public void ToggleFlip()
    {
        if (IsFlipping())
            return;

        isFlippedLeft = !isFlippedLeft;

        if (flipDuration > 0.0f)
        {
            flipTimer = 0.0f;
        }
        else
        {
            SetButtonPosition();
        }
    }

    public void SetFlip(bool setFlipLeft)
    {
        if (setFlipLeft != isFlippedLeft)
        {
            ToggleFlip();
        }
    }

    private void UpdateButtonPosition()
    {
        SetButtonPosition();
    }

    private void SetButtonPosition(Vector3? optionalPosition = null)
    {
        Vector3 newPosition = Vector3.zero;
        if (optionalPosition != null)
        {
            newPosition = optionalPosition.Value;
        }
        else
        {
            float rightDirection = CalculateCoordinate(false) - CalculateCoordinate(true);
            float animationProgress = flipTimer / flipDuration; // Linear
            if (flipCurve.length >= 2) // Modified by animation curve if it meets the [0, 1] criteria
            {
                if (flipCurve[0].time == 0.0f && flipCurve[flipCurve.length - 1].time == 1.0f)
                {
                    animationProgress = flipCurve.Evaluate(animationProgress);
                }
            }
            float animationParameter = IsFlippedLeft() ? 1.0f - animationProgress : animationProgress;

            newPosition.x = CalculateCoordinate(true) + animationParameter * rightDirection;
        }
        toggleButton.GetComponent<RectTransform>().anchoredPosition = newPosition;
    }

    private float CalculateCoordinate(bool isLeft)
    {
        Vector2 slotSize = toggleSlotBackground.rectTransform.sizeDelta;
        if (isLeft)
        {
            return -(slotSize.x / 4f);      
        }
        else
        {
            return (slotSize.x / 4f);
        }
    }

    private bool IsFlipping()
    {
        if (flipTimer >= 0.0f && flipTimer < flipDuration)
            return true;
        else
            return false;
    }
}
