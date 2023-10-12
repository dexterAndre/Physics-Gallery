using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUICollapseToggle))]
public class GUIRotatingToggle : MonoBehaviour
{
    private GUICollapseToggle collapseComponent;
    [SerializeField] private float rotationUntoggled = 0f;
    [SerializeField] private float rotationToggled = 90f;
    [SerializeField] private Image rotationTarget;



    private void Awake()
    {
        collapseComponent = GetComponent<GUICollapseToggle>();
        if (collapseComponent == null)
        {
            Debug.LogError("Error: Could not find GUICollapseToggle. Disabling GUIRotatingToggle.");
            enabled = false;
        }
        if (rotationTarget == null)
        {
            Debug.LogError("Error: Could not find Image component to rotate. Disabling GUIRotatingToggle.");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (collapseComponent != null)
        {
            collapseComponent.onResizeEvent += UpdateRotation;
        }
    }

    private void OnDisable()
    {
        if (collapseComponent != null)
        {
            collapseComponent.onResizeEvent -= UpdateRotation;
        }
    }

    private void UpdateRotation()
    {
        float rotationAngle = rotationToggled + collapseComponent.AnimationParameter * (rotationUntoggled - rotationToggled);
        Vector3 newRotation = new Vector3(0f, 0f, rotationAngle);
        rotationTarget.rectTransform.localRotation = Quaternion.Euler(newRotation);
    }
}
