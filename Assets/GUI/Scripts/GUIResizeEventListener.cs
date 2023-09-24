using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static ManagerPointSet;

[RequireComponent(typeof(RectTransform))]
public class GUIResizeEventListener : UIBehaviour
{
    private RectTransform rectTransform;
    public delegate void GUIEventSignature();
    public event GUIEventSignature onResizeEvent;



    protected override void Awake()
    {
        rectTransform = (RectTransform)transform;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        onResizeEvent?.Invoke();
    }
}
