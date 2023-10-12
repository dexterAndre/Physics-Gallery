using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUICollapseToggle : MonoBehaviour
{
    [System.Serializable]
    public enum ResizeDirection
    {
        Width,
        Height
    }

    // Fucntionality
    [SerializeField] private ResizeDirection resizeDirection = ResizeDirection.Height;
    [SerializeField] private float minimumSize = 0f;
    private Vector2 intermediateSize = Vector2.zero;    // 0 means this value won't get used
    public Vector2 IntermediateSize
    {
        get
        {
            return intermediateSize;
        }
        set
        {
            intermediateSize.x = value.x < 0f ? 0f : value.x;
            intermediateSize.y = value.y < 0f ? 0f : value.y;
        }
    }
    [SerializeField] private bool reverseState = false;
    [SerializeField] private bool isCollapsed = false;
    [SerializeField] private float animationDuration = 0.1f;
    private float animationTimer = 0.1f;
    private float animationProgress = 0.0f;
    private float animationParameter = 0.0f;
    public float AnimationParameter { get { return animationParameter; } }
    [SerializeField] private AnimationCurve animationCurve;
    public AnimationCurve CollapseAnimationCurve { get { return animationCurve; } }
    private bool firstFrame = false;    // Needed to let GUI sizes be drawn before initiualizing values dependent on these

    // References
    [SerializeField] private GameObject collapsiblePanel;
    private Vector2 collapsiblePanelInitSize;
    [SerializeField] private RectTransform parentLayout;
    [SerializeField] private List<AspectRatioFitter> aspectFitters = new List<AspectRatioFitter>();
    [SerializeField] private bool isGrandParent = false;
    [SerializeField] private GUICollapseToggle parentCollapseToggle;
    [SerializeField] private List<GUICollapseToggle> collapseToggles = new List<GUICollapseToggle>();

    // Events
    public delegate void GUIEventSignature();
    public event GUIEventSignature onResizeEvent;



    private void Awake()
    {
        if (animationDuration < 0.0f)
        {
            Debug.LogWarning("Warning: Rotation animation duration is less than zero. Forcing it to be 0, which means an instant animation.");
        }

        if (parentLayout == null)
        {
            Debug.Log("Note: parentLayout RectTransform not found. May result in out-of-date layout group calculations.");
        }
    
        animationTimer = animationDuration;
        SetAnimationParameter();
        firstFrame = true;
    }

    private void Update()
    {
        if (animationDuration > 0.0f && animationTimer < animationDuration)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer > animationDuration)
            {
                FinishTimer();
            }

            SetAnimationParameter();
            SetCollapsiblePanelSize();
            onResizeEvent?.Invoke();
        }
    }

    private void OnGUI()
    {
        if (firstFrame)
        {
            if (collapsiblePanel != null)
            {
                SetLastKnownPanelSize();
                SetCollapsiblePanelSize();
            }
            firstFrame = false;
        }
    }

    public void StartTimer()
    {
        // isCollapsed has been set before the timer starts, so the current isCollapsed value is the target value after the transition
        if (isCollapsed)
        {
            //SetLastKnownPanelSize();
        }

        animationTimer = 0.0f;
        bool isParentConnected = aspectFitters.Count == collapseToggles.Count;

        if (aspectFitters.Count > 0)
        {
            for (int i = 0; i < aspectFitters.Count; i++)
            {
                if (!isGrandParent)
                {
                    aspectFitters[i].enabled = false;
                }
                else if (isParentConnected)
                {
                    aspectFitters[i].enabled = !collapseToggles[i].isCollapsed;
                }
            }
        }
    }

    private void FinishTimer()
    {
        animationTimer = animationDuration;
        bool isParentConnected = aspectFitters.Count == collapseToggles.Count;
        if (aspectFitters.Count > 0)
        {
            for (int i = 0; i < aspectFitters.Count; i++)
            {
                if (!isGrandParent)
                {
                    aspectFitters[i].enabled = !isCollapsed;
                }
                else if (isParentConnected)
                {
                    aspectFitters[i].enabled = !collapseToggles[i].isCollapsed;
                }
            }
        }

        if (parentCollapseToggle != null)
        {
            parentCollapseToggle.SetIntermediateSize(collapsiblePanel.GetComponent<RectTransform>().rect.size);
        }
    }

    public bool IsCollapsed()
    {
        return isCollapsed;
    }

    public bool IsExpanded()
    {
        return !IsCollapsed();
    }

    public void ToggleCollapsed()
    {
        if (IsResizing())
            return;

        isCollapsed = !isCollapsed;

        if (animationDuration > 0.0f)
        {
            StartTimer();
        }
        else
        {
            if (collapsiblePanel == null)
            {
                SetCollapsiblePanelSize();
            }

        }
    }

    public void SetCollapsed(bool setCollapsed)
    {
        if (setCollapsed != isCollapsed)
        {
            ToggleCollapsed();
        }
    }

    private void SetAnimationParameter()
    {
        animationProgress = animationTimer / animationDuration;   // Linear
        if (animationCurve.length >= 2) // Modified by animation curve if it meets the [0, 1] criteria
        {
            if (animationCurve[0].time == 0.0f && animationCurve[animationCurve.length - 1].time == 1.0f)
            {
                animationProgress = animationCurve.Evaluate(animationProgress);
            }
        }

        if (IsCollapsed())
        {
            if (reverseState)
            {
                animationParameter = animationProgress;
            }
            else
            {
                animationParameter = 1.0f - animationProgress;
            }
        }
        else
        {
            if (reverseState)
            {
                animationParameter = 1.0f - animationProgress;
            }
            else
            {
                animationParameter = animationProgress;
            }
        }
    }

    private void SetCollapsiblePanelSize()
    {
        if (collapsiblePanel == null)
            return;

        Vector2 size = IntermediateSize == Vector2.zero ? collapsiblePanelInitSize : IntermediateSize;

        float newValue = minimumSize + animationParameter * ((resizeDirection == ResizeDirection.Width ? size.x : size.y) - minimumSize);
        RectTransform rt = collapsiblePanel.GetComponent<RectTransform>();
        Vector2 newSize = resizeDirection == ResizeDirection.Width ? new Vector2(newValue, size.y) : new Vector2(size.x, newValue);
        rt.sizeDelta = newSize;

        // Forcing parent layout group to redraw
        if (parentLayout != null)
        {
            LayoutRebuilder.MarkLayoutForRebuild(parentLayout);
        }
    }

    private void SetLastKnownPanelSize()
    {
        collapsiblePanelInitSize = collapsiblePanel.GetComponent<RectTransform>().rect.size;
    }

    public void SetIntermediateSize(Vector2 size)
    {
        IntermediateSize = size;
    }

    public bool IsResizing()
    {
        if (animationTimer >= 0.0f && animationTimer < animationDuration)
            return true;
        else
            return false;
    }
}
