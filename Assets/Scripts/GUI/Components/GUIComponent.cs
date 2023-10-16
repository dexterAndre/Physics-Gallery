using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GUIComponent : MonoBehaviour
{
    [SerializeField] private GUIController_Header header;
    [SerializeField] private Transform componentParent;
    [SerializeField] private Image separatorBar;
    [SerializeField] private List<GUIController> guiControllers = new List<GUIController>();
    [SerializeField] private List<TMP_Text> labels = new List<TMP_Text>();
    [SerializeField] private float elementHeight = 24;
    protected PointBehavior_Animate behavior;
    public PointBehavior_Animate Behavior { get { return behavior; } set { behavior = value; } }



    public void Awake()
    {
        if (header == null)
        {
            header = transform.GetChild(0).GetComponent<GUIController_Header>();
        }
        if (componentParent == null)
        {
            componentParent = transform.GetChild(1);
            if (componentParent == null)
            {
                Debug.LogError("Could not find componentParent. Disabling component.");
                gameObject.SetActive(false);
                return;
            }
        }
        if (separatorBar == null)
        {
            separatorBar = transform.GetChild(2).GetComponent<Image>();
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        SetComponentContentsHeight();

        yield return null;
    }

    private void SetComponentContentsHeight()
    {
        AspectRatioFitter aspectFitter = componentParent.GetComponent<AspectRatioFitter>();
        aspectFitter.enabled = false;
        RectTransform rt = componentParent.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.rect.width, elementHeight * componentParent.childCount);
        aspectFitter.aspectRatio = rt.sizeDelta.x / rt.sizeDelta.y;
        aspectFitter.enabled = true;
    }

    public void ApplyColorPalette(ColorPalette palette)
    {
        header.ApplyColorPalette(palette);
        EditorUtility.SetDirty(header);
        separatorBar.color = palette.colorForegroundFill;
        EditorUtility.SetDirty(separatorBar);
        foreach (GUIController controller in guiControllers)
        {
            controller.ApplyColorPalette(palette);
        }
        foreach (TMP_Text text in labels)
        {
            text.color = palette.colorFont;
            EditorUtility.SetDirty(text);
        }
    }
}
