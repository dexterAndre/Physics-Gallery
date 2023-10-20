using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [Header("Camera Controls")]
    [SerializeField] private float mouseAcceleration = 100f;
    [SerializeField] private float mouseLerpSpeed = 0.1f;
    [SerializeField] private float distanceMargin = 0.1f;       // Percentage added on top of Distance
    public float Distance
    {
        get
        {
            return coordinateSystem.BoundsHalf.magnitude * (1f + distanceMargin);
        }
    }
    [SerializeField] private Vector2 pitchBounds = new Vector2(-89f, 89f);
    [SerializeField, Tooltip("Multiplier for inverting mouse controls (x/y) on each individual axis. 1 for normal, -1 for inverse.")]
    private Vector2Int inverseMouseControls = new Vector2Int(1, 1);
    private Vector2 mouseVelocity = Vector2.zero;
    private Vector2 mouseCoordinatePrevious = Vector2.zero;
    private float pitch = 0f;
    private float initPitch = 0f;
    private float yaw = MathUtils.TAU / 4f;
    private float initYaw = MathUtils.TAU / 4f;
    [SerializeField] private float resetDuration = 0.25f;
    private float resetTimer = 0f;
    [SerializeField] private AnimationCurve resetCurve; 
    private float preResetPitch = 0f;
    private float preResetYaw = 0f;
    [SerializeField] private AnimationCurve mouseAxisBias;
    [SerializeField] private uint mouseAxisBiasPower = 1;
    private bool isMouseOverGUI = false;
    private bool startedClickOnGUI = false;

    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private CoordinateSystem coordinateSystem;
    [SerializeField] private List<PointerEventReporter> guiExclusionList = new List<PointerEventReporter>();

    [Header("Debug")]
    [SerializeField] private bool debug = false;



    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("Error: Could not find camera. Disabling component.");
                enabled = false;
                return;
            }
        }

        if (coordinateSystem == null)
        {
            coordinateSystem = transform.GetComponentInChildren<CoordinateSystem>();
            if (coordinateSystem == null)
            {
                Debug.LogError("Error: Could not find CoordinateOverlayCartesian. Disabling component.");
                enabled = false;
                return;
            }
        }

        mouseVelocity = Vector2.zero;
        initPitch = pitch;
        initYaw = yaw;
    }

    private void OnEnable()
    {
        foreach (PointerEventReporter pointerEventReporter in guiExclusionList)
        {
            pointerEventReporter.onPointerEnter += OnMouseEnterGUI;
            pointerEventReporter.onPointerExit += OnMouseExitGUI;
        }
    }

    private void OnDisable()
    {
        foreach (PointerEventReporter pointerEventReporter in guiExclusionList)
        {
            pointerEventReporter.onPointerEnter -= OnMouseEnterGUI;
            pointerEventReporter.onPointerExit -= OnMouseExitGUI;
        }
    }

    private void LateUpdate()
    {
        ConditionalUpdateResetTimer();

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (isMouseOverGUI)
            {
                startedClickOnGUI = true;
            }
            else
            {
                startedClickOnGUI = false;
            }

            mouseCoordinatePrevious = (Vector2)Input.mousePosition;
        }

        Vector2 mouseDelta = Vector2.zero;

        // Left-click: rotates camera on hold, floats the camera in direction of mouse (with mouse velocity) on release
        if (Input.GetMouseButton(0) && !IsResetting() && !startedClickOnGUI)
        {
            mouseDelta = mouseCoordinatePrevious - (Vector2)Input.mousePosition;
            mouseDelta = new Vector2(mouseDelta.x * inverseMouseControls.x, mouseDelta.y * inverseMouseControls.y);
            mouseDelta = ApplyAxisBias(mouseDelta, mouseAxisBiasPower);
            mouseVelocity += mouseDelta * mouseAcceleration * Time.deltaTime;
        }

        // Gravitates towards zero
        if (!IsResetting())
        {
            mouseVelocity = Vector2.Lerp(mouseVelocity, Vector2.zero, mouseLerpSpeed * Time.deltaTime);
            pitch += mouseVelocity.y * Time.deltaTime;
            if (pitch < pitchBounds.x * Mathf.Deg2Rad)
                pitch = pitchBounds.x * Mathf.Deg2Rad;
            else if (pitch > pitchBounds.y * Mathf.Deg2Rad)
                pitch = pitchBounds.y * Mathf.Deg2Rad;
            yaw -= mouseVelocity.x * Time.deltaTime;
            if (yaw > MathUtils.TAU)
                yaw -= MathUtils.TAU;
            else if (yaw < 0f)
                yaw += MathUtils.TAU;
        }

        // Right-click: initiates reset animation
        if (Input.GetMouseButton(1) && !IsResetting() && !startedClickOnGUI)
        {
            resetTimer += Time.deltaTime;
            preResetPitch = pitch;
            preResetYaw = yaw;
            mouseVelocity = Vector2.zero;
        }

        // TODO: Clean up conditionals
        if (IsResetting())
        {
            // TODO: Fix snap orientation when yaw ~= TAU, find out shortest path instead
            pitch = Mathf.Lerp(preResetPitch, initPitch, resetCurve.Evaluate(ResetProgress()));
            yaw = Mathf.Lerp(preResetYaw, initYaw, resetCurve.Evaluate(ResetProgress()));
        }

        cam.transform.position = CalculateCameraPosition(pitch, yaw, Distance);
        cam.transform.LookAt(Vector3.zero, Vector3.up);

        // Caching current frame info as previous frame for next frame
        mouseCoordinatePrevious = Input.mousePosition;

        if (debug)
        {
            Debug.DrawLine(Vector3.zero, mouseDelta, Color.red);
            Debug.DrawLine(Vector3.zero, mouseVelocity, Color.green);
            Debug.DrawLine(Vector3.zero, CalculateCameraPosition(pitch, yaw, Distance), Color.black);
        }
    }

    private Vector2 ApplyAxisBias(Vector2 input, uint power = 1)
    {
        Vector2 inputNormalized = input.normalized;
        float biasX = Mathf.Pow(mouseAxisBias.Evaluate(Mathf.Abs(inputNormalized.x)), power);
        float biasY = Mathf.Pow(mouseAxisBias.Evaluate(Mathf.Abs(inputNormalized.y)), power);
        input = new Vector2(
            biasX * input.x,
            biasY * input.y);

        return input;
    }

    public Vector3 CalculateCameraPosition(float inPitch, float inYaw, float inDistance)
    {
        float cosY = Mathf.Cos(inYaw);
        float cosP = Mathf.Cos(inPitch);
        float sinY = Mathf.Sin(inYaw);
        float sinP = Mathf.Sin(inPitch);
        return inDistance * new Vector3(
            cosY * cosP,
            sinP,
            -sinY * cosP);
    }

    private bool IsResetting()
    {
        return resetTimer > 0;
    }

    private void ConditionalUpdateResetTimer()
    {
        if (resetTimer <= 0)
            return;

        resetTimer += Time.deltaTime;
        if (resetTimer > resetDuration)
        {
            resetTimer = 0f;
            preResetPitch = 0f;
            preResetYaw = 0f;
        }
    }

    private float ResetProgress()
    {
        return resetTimer / resetDuration;
    }

    private void OnMouseEnterGUI()
    {
        isMouseOverGUI = true;
    }

    private void OnMouseExitGUI()
    {
        isMouseOverGUI = false;
    }
}
