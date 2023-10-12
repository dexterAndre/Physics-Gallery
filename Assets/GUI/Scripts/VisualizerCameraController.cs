using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class VisualizerCameraController : MonoBehaviour
{
    // Settings
    [SerializeField] Camera cam;
    [SerializeField, Tooltip("Padding added between content and end of the screen, preserving an aspect ratio of 1.")] private float padding = 0.15f;
    public float Padding
    {
        get { return padding; }
        set { padding = value; }
    }
    [SerializeField, Tooltip("Size in world space for content to exist in.")]
    private Vector3Int boundarySpan = Vector3Int.one;
    public Vector3Int BoundarySpan
    {
        get { return boundarySpan; }
        set
        {
            boundarySpan = new Vector3Int(
                Mathf.Max(value.x, 1),
                Mathf.Max(value.y, 1),
                Mathf.Max(value.z, 1));
            RecalculateCameraSize();
        }
    }
    [SerializeField, Tooltip("Maximum content size before scaling camera size based on it.")]
    private int maxScreenSizeForContentScaling = 1080;
    public int MaxScreenSizeForContentScaling { get { return maxScreenSizeForContentScaling; } }

    // Camera resizing cache
    private int currentCameraWidth;
    private int currentCameraHeight;
    private int previousCameraWidth;
    private int previousCameraHeight;

    // Events
    public delegate void CameraEventSignature();
    public event CameraEventSignature onResizeEvent;



    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        InitializeCameraSizes();
        onResizeEvent?.Invoke();
    }

    private void OnEnable()
    {
        onResizeEvent += RecalculateCameraSize;

        InitializeCameraSizes();
    }

    private void OnDisable()
    {
        onResizeEvent -= RecalculateCameraSize;
    }

    private void OnValidate()
    {
        BoundarySpan = boundarySpan;    // Shortcut to force editor assignment to run its setter function
        InitializeCameraSizes();
        onResizeEvent?.Invoke();
    }

    private void OnGUI()
    {
        if (cam == null)
            return;

        currentCameraWidth = cam.pixelWidth;
        currentCameraHeight = cam.pixelHeight;
        if (WasCameraResized())
        {
            previousCameraWidth = currentCameraWidth;
            previousCameraHeight = currentCameraHeight;
            onResizeEvent?.Invoke();
        }
    }

    public void RecalculateCameraSize()
    {
        // TODO: Support 3D as well
        float boundsAspectRatio = (float)BoundarySpan.x / (float)BoundarySpan.y;
        float cameraSize = 1f;
        if (boundsAspectRatio > cam.aspect)
        {
            cameraSize = BoundarySpan.x / cam.aspect;
        }
        else
        {
            cameraSize = BoundarySpan.y;
        }

        cameraSize = (cameraSize * 0.5f) / (1f - padding);

        Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height);
        float minScreenSize = Mathf.Min(screenSize.x, screenSize.y);
        if (minScreenSize > maxScreenSizeForContentScaling)
        {
            cameraSize *= (minScreenSize / maxScreenSizeForContentScaling);
        }

        cam.orthographicSize = cameraSize;
    }

    private bool WasCameraResized()
    {
        return (currentCameraWidth != previousCameraWidth || currentCameraHeight != previousCameraHeight);
    }

    private void InitializeCameraSizes()
    {
        currentCameraWidth = cam.pixelWidth;
        currentCameraHeight = cam.pixelHeight;
        previousCameraWidth = currentCameraWidth;
        previousCameraHeight = currentCameraHeight;
    }
}
