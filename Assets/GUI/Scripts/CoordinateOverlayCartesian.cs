using Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CoordinateOverlayCartesian : MonoBehaviour
{
    private struct Vector3Pair
    {
        public Vector3Pair(Vector3 v1, Vector3 v2)
        {
            A = v1;
            B = v2;
        }
        public Vector3Pair(Vector3 position, Vector3 direction, float magnitude)
        {
            A = position;
            B = position + direction.normalized * magnitude;
        }

        public Vector3 A;
        public Vector3 B;
    }

    private enum Direction
    {
        East,
        North,
        West,
        South
    }

    [System.Serializable]
    private enum MarkerType
    {
        Gridlines,
        Crosshairs,
        Circles
    }

    [Header("Settings")]
    [SerializeField] private int subdivisions = 2;
    private int Subdivisions
    {
        get { return subdivisions; }
        set
        {
            subdivisions = value;
        }
    }
    [SerializeField] private Vector3Int bounds = Vector3Int.one;
    public Vector3Int Bounds
    {
        get { return bounds; }
        set
        {
            bounds = value;
            cameraController.BoundarySpan = bounds;
        }
    }

    [Header("Tick Markers")]
    [SerializeField] private bool showTickMarkers = true;
    [SerializeField, Tooltip("Min and Max length for ticks.")]
    Vector2 tickMarkerLength = new Vector2(0.05f, 0.05f);
    private Vector2 TickMarkerLength
    {
        get { return tickMarkerLength; }
        set
        {
            tickMarkerLength = value;
        }
    }
    [SerializeField, Tooltip("Does tickLength represent percentage of bounds?")]
    private bool relativeMarkerLength = false;
    [SerializeField] private bool halfSkewTickPattern = true;
    [SerializeField] private float tickMarkerThickness = 1f;

    [Header("Grid Markers")]
    [SerializeField] private bool showGridMarkers = true;
    public bool ShowGridMarkers
    {
        get { return showGridMarkers; }
        set
        {
            showGridMarkers = value;
            transform.Find("Grid").gameObject.SetActive(showGridMarkers);
        }
    }
    [SerializeField] private bool includeEdgeGridMarkers = false;
    [SerializeField] private MarkerType gridMarkerType = MarkerType.Crosshairs;
    [SerializeField, Tooltip("Adjusts lenght of grid crosshairs or grid circles. Does not affect grid lines.")]
    private float gridMarkerLengthMultiplier = 0.25f;
    [SerializeField] private float gridMarkerThickness = 1f;

    [Header("Misc.")]
    [SerializeField, Tooltip("Screen-space percentage of how much title menu is offset from the top of the grid. Acts as a multiplier if titleMenuPaddingRelative is true.")]
    float titleMenuPaddingOffset = 0.1f;
    [SerializeField] bool titleMenuPaddingRelative = false;
    [SerializeField]
    private float fontSizeMultiplier = 0.1f;

    [Header("References")]
    [SerializeField] private VisualizerCameraController cameraController;
    [SerializeField, Tooltip("Boundary visualizer. Also used to reference graphics settings from, for example in subobjects like ticks or points of interest.")]
    private Polyline gridCorners;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject doubleLineObject;
    [SerializeField] private GameObject circleObject;
    [SerializeField] private GameObject titleMenu;



    private void Awake()
    {
        if (cameraController == null)
        {
            transform.parent.GetComponent<VisualizerCameraController>();
            if (cameraController == null)
            {
                Debug.LogError("Error: Could not find parent VisualizerCameraController. It is needed for calculations. Disabling this CoordinateOverlayCartesian component.");
                enabled = false;
            }
        }

        if (gridCorners == null)
        {
            gridCorners = GetComponent<Polyline>();
        }

        RecalculateCoordinateSystem();
    }

    private void OnEnable()
    {
        if (cameraController != null)
        {
            cameraController.onResizeEvent += RecalculateCoordinateSystem;
        }
    }

    private void OnDisable()
    {
        if (cameraController != null)
        {
            cameraController.onResizeEvent -= RecalculateCoordinateSystem;
        }
    }

    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () =>
        {
            RecalculateCoordinateSystem();
        };
    }

    private void OnDestroy()
    {
        DestroyAllChildren();
    }

    public void RecalculateCoordinateSystem()
    {
        cameraController.BoundarySpan = Bounds;
        SetGridCorners();
        StartCoroutine(StartRecreatingMarkers());
        SetTitleMenuPosition();
    }

    private void SetGridCorners()
    {
        if (gridCorners == null)
        {
            return;
        }

        gridCorners.points.Clear();
        Vector3 refVec = ((Vector3)Bounds) / 2f;
        gridCorners.AddPoint(new Vector3(refVec.x, refVec.y, 0f));
        gridCorners.AddPoint(new Vector3(-refVec.x, refVec.y, 0f));
        gridCorners.AddPoint(new Vector3(-refVec.x, -refVec.y, 0f));
        gridCorners.AddPoint(new Vector3(refVec.x, -refVec.y, 0f));
    }

    private void SetTitleMenuPosition()
    {
        // Old code: consider deleting
        //float paddingOffset = titleMenuPaddingRelative ? titleMenuPaddingOffset * cameraController.Padding : titleMenuPaddingOffset;
        //float yPosition = Camera.main.orthographicSize * (1f - cameraController.Padding + paddingOffset);
        //titleMenu.transform.position = new Vector3(0f, yPosition, 0f);

        float yPosition = gridCorners[0].point.y * (1 + cameraController.Padding);
        titleMenu.transform.position = new Vector3(0f, yPosition, 0f);
        titleMenu.transform.GetChild(1).GetComponent<TMP_Text>().fontSize = fontSizeMultiplier * Camera.main.orthographicSize;
    }

    public IEnumerator StartRecreatingMarkers()
    {
        // Clears previous ticks
        DestroyAllChildren();
        yield return new WaitForEndOfFrame();

        if (subdivisions >= 0)
        {
            SetGridCorners();
            if (showTickMarkers)
            {
                RecreateTickMarkers();
            }
            if (showGridMarkers)
            {
                RecreateGridMarkers(gridMarkerType);
            }
        }
    }

    private void RecreateTickMarkers()
    {
        // Creates new tick gameObjects
        Transform eastParent = transform.Find("East");
        Transform northParent = transform.Find("North");
        Transform westParent = transform.Find("West");
        Transform southParent = transform.Find("South");
        int tickCountHorizontal = CalculateTickCount(subdivisions, Bounds.x);
        int tickCountVertical = CalculateTickCount(subdivisions, Bounds.y);
        CreateTickMarkerChildren(tickCountVertical, Direction.East, eastParent);
        CreateTickMarkerChildren(tickCountHorizontal, Direction.North, northParent);
        CreateTickMarkerChildren(tickCountVertical, Direction.West, westParent);
        CreateTickMarkerChildren(tickCountHorizontal, Direction.South, southParent);
    }

    private void RecreateGridMarkers(MarkerType type)
    {
        // Creates new marker gameObjects
        Transform gridParent = transform.Find("Grid");
        CreateGridMarkerChildren(type, gridParent);
    }

    private GameObject CreateMarkerChild(GameObject prefab, Transform parent, Vector3 position = default, Quaternion rotation = default)
    {
        return Instantiate(prefab, position, rotation, parent);
    }

    private void CreateTickMarkerChildren(int tickCount, Direction side, Transform tickParent)
    {
        for (int i = 0; i < tickCount; i++)
        {
            float linearProgress = (float)i / (tickCount - 1);
            GameObject child = CreateMarkerChild(lineObject, tickParent);
            Vector3Pair cornerPoints = SelectGridCorners(side);
            Vector3 position = cornerPoints.A + linearProgress * (cornerPoints.B - cornerPoints.A);
            int skewTickPatternOffset = halfSkewTickPattern ? ((tickCount - 1) / 2) : 0;
            float magnitude = CalculateTickLength(tickMarkerLength.x, tickMarkerLength.y, i, subdivisions, skewTickPatternOffset);
            bool isLateral = side == Direction.East || side == Direction.West;
            if (relativeMarkerLength)
            {
                magnitude = magnitude / (isLateral ? Bounds.x : Bounds.y);
            }
            if (isLateral)
            {
                child.name = "Tick (x = " + linearProgress.ToString() + ", y = " + SelectSideDirection(side).y + ")";
            }
            else
            {
                child.name = "Tick (x = " + SelectSideDirection(side).x + ", y = " + linearProgress.ToString() + ")";
            }
            Vector3 direction = SelectDirectionVector(side);
            Vector3Pair linePoints = new Vector3Pair(position, direction, magnitude);
            Line line = child.GetComponent<Line>();
            line.Start = linePoints.A;
            line.End = linePoints.B;
            line.Thickness = tickMarkerThickness;
        }
    }

    private void CreateGridMarkerChildren(MarkerType type, Transform gridParent)
    {
        // TODO: Implement 3D
        // Corner points, starting from north-east, going counter-clockwise
        Vector3 NE = gridCorners[0].point;
        Vector3 NW = gridCorners[1].point;
        Vector3 SW = gridCorners[2].point;
        Vector3 SE = gridCorners[3].point;
        Vector3 rightInterval = SE - SW;
        Vector3 rightNormalized = rightInterval.normalized;
        Vector3 upInterval = NW - SW;
        Vector3 upNormalized = upInterval.normalized;
        int tickCountUnitInterval = CalculateTickCount(subdivisions, Bounds.x);
        int tickCountX = CalculateTickCount(subdivisions, Bounds.x);
        int tickCountY = CalculateTickCount(subdivisions, Bounds.y);
        int startIndex = includeEdgeGridMarkers ? 0 : 1;
        int endIndexX = includeEdgeGridMarkers ? tickCountX : tickCountX - 1;
        int endIndexY = includeEdgeGridMarkers ? tickCountY : tickCountY - 1;

        if (type == MarkerType.Gridlines)
        {
            // Separating horizontal gridlines from vertical gridlines maintains a nice order in the hierarchy
            // Horizontal gridlines
            for (int i = startIndex; i < endIndexX; i++)
            {
                float linearProgress = (float)i / (tickCountX - 1);
                GameObject childHorizontal = CreateMarkerChild(lineObject, gridParent);
                childHorizontal.isStatic = true;
                childHorizontal.name = "Gridline (x = " + (linearProgress * Bounds.x * 2 - Bounds.x).ToString() + ")";
                Vector3 positionIteratorHorizontal = SW + linearProgress * rightInterval;
                Line lineHorizontal = childHorizontal.GetComponent<Line>();
                lineHorizontal.Start = positionIteratorHorizontal;
                lineHorizontal.End = positionIteratorHorizontal + upInterval;
                lineHorizontal.Thickness = gridMarkerThickness;
            }
            // Vertical gridlines
            for (int i = startIndex; i < endIndexY; i++)
            {
                float linearProgress = (float)i / (tickCountY - 1);
                GameObject childVertical = CreateMarkerChild(lineObject, gridParent);
                childVertical.isStatic = true;
                childVertical.name = "Gridline (y = " + (linearProgress * Bounds.y * 2 - Bounds.y).ToString() + ")";
                Vector3 positionIteratorVertical = SW + linearProgress * upInterval;
                Line lineVertical = childVertical.GetComponent<Line>();
                lineVertical.Start = positionIteratorVertical;
                lineVertical.End = positionIteratorVertical + rightInterval;
                lineVertical.Thickness = gridMarkerThickness;
            }
        }
        else if (type == MarkerType.Crosshairs || type == MarkerType.Circles)
        {
            // Same generation procedure, but different prefabs
            GameObject selectedPrefab;
            if (type == MarkerType.Crosshairs)
                selectedPrefab = doubleLineObject;
            else if (type == MarkerType.Circles)
                selectedPrefab = circleObject;
            else
            {
                Debug.LogWarning("Warning: Could not select prefab to spawn. Either the references object was null or there is a logical error when differentiating on MarkerType.");
                return;
            }
            string childName = type == MarkerType.Crosshairs ? "Crosshair" : "Circle";

            for (int y = startIndex; y < endIndexY; y++)
            {
                for (int x = startIndex; x < endIndexX; x++)
                {
                    float linearProgressX = (float)x / (tickCountX - 1);
                    float linearProgressY = (float)y / (tickCountY - 1);
                    GameObject child = CreateMarkerChild(selectedPrefab, gridParent);
                    child.isStatic = true;
                    // TODO: Fix naming!
                    child.name = childName + " (x = " + (linearProgressX * Bounds.x * 2 - Bounds.x).ToString() + ", y = " + (linearProgressY * Bounds.y * 2 - Bounds.y) + ")";
                    Vector3 position = SW + linearProgressX * rightInterval + linearProgressY * upInterval;
                    child.transform.position = position;
                    if (type == MarkerType.Crosshairs)
                    {
                        Line lineHorizontal = child.transform.GetChild(0).GetComponent<Line>();
                        Line lineVertical = child.transform.GetChild(1).GetComponent<Line>();
                        // TODO: Generalize this better
                        lineHorizontal.Start = -rightNormalized * TickMarkerLength.y * gridMarkerLengthMultiplier;
                        lineHorizontal.End = rightNormalized * TickMarkerLength.y * gridMarkerLengthMultiplier;
                        lineHorizontal.Thickness = gridMarkerThickness;
                        lineVertical.Start = -upNormalized * TickMarkerLength.y * gridMarkerLengthMultiplier;
                        lineVertical.End = upNormalized * TickMarkerLength.y * gridMarkerLengthMultiplier;
                        lineVertical.Thickness = gridMarkerThickness;
                    }
                    else if (type == MarkerType.Circles)
                    {
                        Disc disc = child.GetComponent<Disc>();
                        disc.Radius = TickMarkerLength.y * gridMarkerLengthMultiplier;
                    }
                }
            }
        }
    }

    private Vector3 SelectDirectionVector(Direction side)
    {
        switch (side)
        {
            case Direction.East:
            {
                return Vector3.right;
            }
            case Direction.North:
            {
                return Vector3.up;
            }
            case Direction.West:
            {
                return Vector3.left;
            }
            case Direction.South:
            {
                return Vector3.down;
            }
            default:
            {
                return Vector3.zero;
            }
        }
    }

    private Vector3Pair SelectGridCorners(Direction side)
    {
        Vector3Pair points = new Vector3Pair(Vector3.zero, Vector3.zero);

        switch (side)
        {
            case Direction.East:
            {
                points.A = gridCorners[0].point;
                points.B = gridCorners[3].point;
                break;
            }
            case Direction.North:
            {
                points.A = gridCorners[1].point;
                points.B = gridCorners[0].point;
                break;
            }
            case Direction.West:
            {
                points.A = gridCorners[2].point;
                points.B = gridCorners[1].point;
                break;
            }
            case Direction.South:
            {
                points.A = gridCorners[3].point;
                points.B = gridCorners[2].point;
                break;
            }
            default:
            {
                break;
            }
        }

        return points;
    }

    private Vector3 SelectSideDirection(Direction side)
    {
        Vector3 direction = Vector3.zero;

        switch (side)
        {
            case Direction.East:
            {
                direction = Vector3.right;
                break;
            }
            case Direction.North:
            {
                direction = Vector3.up;
                break;
            }
            case Direction.West:
            {
                direction = Vector3.left;
                break;
            }
            case Direction.South:
            {
                direction = Vector3.down;
                break;
            }
        }

        return direction;
    }

    private float CalculateTickLength(float minLength, float maxLength, int tickIndex, int subdivision, int boundarySize, int patternOffset = 0)
    {
        // Following the general pattern of:
        /*
            [0] ----    mod8 == 0
            [1] -       mod1 == 0
            [2] --      mod2 == 0
            [3] -       mod1 == 0
            [4] ---     mod4 == 0
            [5] -       mod1 == 0
            [6] --      mod2 == 0
            [7] -       mod1 == 0
            [8] ----    mod8 == 0 
        */ 

        float subdivisionDepthFraction = 0f;
        float span = (CalculateTickCount(subdivision, boundarySize) - 1);
        int subdivisionTester = subdivision;
        while (subdivisionTester >= 0)
        {
            // Checking what level of subdivision current tick lies within
            int currentTest = CalculateTickCount(subdivisionTester, 1) - 1;  // TODO: Is this correct?
            if ((tickIndex + patternOffset) % currentTest == 0)
            {
                subdivisionDepthFraction = Mathf.Log(currentTest, span);
                break;
            }
            subdivisionTester--;
        }

        return minLength + subdivisionDepthFraction * (maxLength - minLength);
    }

    private int CalculateTickCount(int subdivision, int boundarySize)
    {
        return Mathf.RoundToInt(Mathf.Pow(2, subdivision)) * boundarySize + 1;
    }

    private void DestroyChildren(Transform parent)
    {
        if (Application.isPlaying)
        {
            for (int childIndex = parent.childCount - 1; childIndex >= 0; childIndex--)
            {
                Destroy(parent.GetChild(childIndex).gameObject);
            }
        }
        else
        {
            for (int childIndex = parent.childCount - 1; childIndex >= 0; childIndex--)
            {
                DestroyImmediate(parent.GetChild(childIndex).gameObject);
            }
        }
    }    

    private void DestroyAllChildren()
    {
        for (int parentIndex = 0; parentIndex < transform.childCount; parentIndex++)
        {
            // Iterating backwards and destroying all grandchildren
            // Note: Must wait until end of frame for these changes to take effect!
            DestroyChildren(transform.GetChild(parentIndex));
        }
    }
}
