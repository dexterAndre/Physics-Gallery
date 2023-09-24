using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class VisualizerCameraController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField, Tooltip("Margin added between content and end of the screen, preserving an aspect ratio of 1.")] private float margin = 0.15f;
    [SerializeField, Tooltip("Size in world space for content to exist in.")]
    private Vector3 boundarySize = Vector3.one;
    public Vector3 BoundarySize { get { return boundarySize; } }
    [SerializeField] Shapes.Polyline coordinateSystemSquare = new Shapes.Polyline();
    [SerializeField] Shapes.Polyline coordinateSystemCircle = new Shapes.Polyline();
    [SerializeField] Shapes.Polyline coordinateSystemSector = new Shapes.Polyline();
    [SerializeField] Shapes.Polyline coordinateSystemCube = new Shapes.Polyline();
    [SerializeField] Shapes.Polyline coordinateSystemSphere = new Shapes.Polyline();
    [SerializeField] Shapes.Polyline coordinateSystemCone = new Shapes.Polyline();



    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        RecalculateCameraSize();
    }

    public void RecalculateCameraSize()
    {
        float maxBoundaryValue = Mathf.Max(boundarySize.x, Mathf.Max(boundarySize.y, boundarySize.z));
        cam.orthographicSize = maxBoundaryValue * (1f + margin);
    }

    private void OnValidate()
    {
        RecalculateCameraSize();
    }
}
