using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventReporter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void PointerEventSignature();
    public event PointerEventSignature onPointerEnter;
    public event PointerEventSignature onPointerExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke();
    }
}
