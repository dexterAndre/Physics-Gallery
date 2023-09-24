using UnityEngine;
using UnityEngine.UI;

public class GUIForceLayoutRebuild : MonoBehaviour
{
    [SerializeField] private RectTransform targetRect;



    private void Awake()
    {
        if (targetRect == null)
        {
            Debug.LogError("Error: targetRect not found. Disabling GUIForceLayoutRebuild component.");
            enabled = false;
        }
    }

    private void ForceLayoutRebuild()
    {
        if (targetRect != null)
        {
            LayoutRebuilder.MarkLayoutForRebuild(targetRect);
        }
    }
}
