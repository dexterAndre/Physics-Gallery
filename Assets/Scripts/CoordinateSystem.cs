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



[ExecuteAlways]
public class CoordinateSystem : ImmediateModeShapeDrawer
{
    #region Structs and enums
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
    #endregion

    [Header("Content Area")]
    [SerializeField] GUIComponent_Settings settingsComponent;
    public bool Is2D
    {
        get
        {
            return settingsComponent.Is2D;
        }
    }
    [SerializeField] public Vector3Int bounds = Vector3Int.one;
    private Vector3Int previousBounds;
    public Vector3 BoundsHalf
    {
        get { return (Vector3)bounds / 2f; }
    }
    [SerializeField, Tooltip("Boundary visualizer. Also used to reference graphics settings from, for example in subobjects like ticks or points of interest.")]
    private List<Vector3> gridCorners = new List<Vector3>();
    public List<Vector3> GridCorners { get { return gridCorners; } }
    public float ContentAreaRadius
    {
        get
        {
            if (gridCorners.Count == 0)
                SetGridCorners();

            return gridCorners[0].magnitude;
        }
    }

    [Header("Screen")]
    [SerializeField, Tooltip("Maximum content size before scaling camera size based on it.")]
    private int maxScreenSizeForContentScaling = 1080;
    public int MaxScreenSizeForContentScaling { get { return maxScreenSizeForContentScaling; } }
    [SerializeField, Range(0f, 1f), Tooltip("Padding added between content and end of the screen, preserving an aspect ratio of 1.")] private float contentAreaPadding = 0.15f;
    public float ContentAreaPadding
    {
        get { return contentAreaPadding; }
        set { contentAreaPadding = value; }
    }
    public bool IsScreenWidthDominant { get { return Camera.main.pixelWidth >= Camera.main.pixelHeight; } }
    private float smallestScreenDimension;
    public float SmallestScreenDimension { get { return smallestScreenDimension; } }
    //Camera resizing cache
    private int currentCameraWidth;
    private int currentCameraHeight;
    private int previousCameraWidth;
    private int previousCameraHeight;

    [Header("Rendering")]
    [SerializeField] private LineGeometry lineGeometry = LineGeometry.Billboard;
    [SerializeField] private ThicknessSpace thicknessSpace = ThicknessSpace.Pixels;
    [SerializeField] private float thickness = 1;

    [Header("Markers")]
    [SerializeField] private int subdivisions = 2;
    private int Subdivisions
    {
        get { return subdivisions; }
        set
        {
            subdivisions = value;
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

    [Header("Menu Settings")]
    [SerializeField, Range(0f, 1f), Tooltip("Screen-space percentage of how much title menu is offset from the top of the grid. Acts as a multiplier if titleMenuPaddingRelative is true.")]
    float titleMenuPaddingOffset = 0.1f;
    [SerializeField] bool titleMenuPaddingRelative = false;
    [SerializeField] private float fontSizeMultiplier = 0.1f;
    [SerializeField] private Vector2 sideMenuOffsetPixels = new Vector2(100f, -100f);
    public Vector2 SideMenuOffsetPixels
    {
        get { return sideMenuOffsetPixels; }
        set
        {
            sideMenuOffsetPixels = value;
            if (sideMenuOffsetPixels.x < 0)
                sideMenuOffsetPixels.x = 0;
            if (sideMenuOffsetPixels.y < 0)
                sideMenuOffsetPixels.y = 0;
        }
    }
    private Vector3 titleMenuPositionCached = Vector3.zero;

    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject doubleLineObject;
    [SerializeField] private GameObject circleObject;
    [SerializeField] private GameObject titleMenu;
    [SerializeField] private GameObject sideMenu;
    [SerializeField] private Camera cam;

    [Header("Debug")]
    [SerializeField] private bool debug = false;

    // Events
    public delegate void CameraEventSignature();
    public event CameraEventSignature onResizeEvent;



    private void Awake()
    {
        if (cam == null)
        { 
            cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("Error: Could not find camera. Disabling this CoordinateOverlayCartesian component.");
                enabled = false;
                return;
            }
        }

        if (cameraController == null)
        {
            transform.parent.GetComponent<CameraController>();
            if (cameraController == null)
            {
                Debug.LogError("Error: Could not find parent VisualizerCameraController. It is needed for calculations. Disabling this CoordinateOverlayCartesian component.");
                enabled = false;
                return;
            }
        }

        previousBounds = bounds;
        InitializeCameraSizes();
        onResizeEvent?.Invoke();
    }

    public override void OnEnable()
    {
        base.OnEnable();


        if (cameraController != null)
        {
            onResizeEvent += RecalculateCoordinateSystem;
        }

        InitializeCameraSizes();        // TODO: Consider if needed
        SetDrawParameters();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (cameraController != null)
        {
            onResizeEvent -= RecalculateCoordinateSystem;
        }
    }

    private void OnValidate()
    {
        if (bounds.x > 0 || bounds.y > 0 || bounds.z > 0)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                RecalculateCoordinateSystem();
            };
        }
    }

    private void OnDestroy()
    {
        DestroyAllChildren();
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

        if (WasContentAreaResized())
        {
            previousBounds = bounds;
            onResizeEvent?.Invoke();
        }
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

    private void SetDrawParameters()
    {
        Draw.LineGeometry = lineGeometry;
        Draw.ThicknessSpace = thicknessSpace;
        Draw.Thickness = thickness;
        Draw.Matrix = transform.localToWorldMatrix;
    }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            // TODO: Customize colors, draw geometry, etc. in monolithic settings struct
            if (Is2D)
            {
                Draw.Line(gridCorners[0], gridCorners[1], Color.white);
                Draw.Line(gridCorners[1], gridCorners[2], Color.white);
                Draw.Line(gridCorners[2], gridCorners[3], Color.white);
                Draw.Line(gridCorners[3], gridCorners[0], Color.white);
            }
            else
            {
                // Close face
                Draw.Line(gridCorners[0], gridCorners[1], Color.white);
                Draw.Line(gridCorners[1], gridCorners[2], Color.white);
                Draw.Line(gridCorners[2], gridCorners[3], Color.white);
                Draw.Line(gridCorners[3], gridCorners[0], Color.white);

                // Far face
                Draw.Line(gridCorners[4], gridCorners[5], Color.white);
                Draw.Line(gridCorners[5], gridCorners[6], Color.white);
                Draw.Line(gridCorners[6], gridCorners[7], Color.white);
                Draw.Line(gridCorners[7], gridCorners[4], Color.white);

                // Connecting faces
                Draw.Line(gridCorners[0], gridCorners[4], Color.white);
                Draw.Line(gridCorners[1], gridCorners[5], Color.white);
                Draw.Line(gridCorners[2], gridCorners[6], Color.white);
                Draw.Line(gridCorners[3], gridCorners[7], Color.white);
            }

            if (debug)
            {
                // Temporary draw parameters - revert after drawing!
                float previousThickness = Draw.Thickness;
                Draw.Thickness = 5f;
                LineEndCap previousLineEndCap = Draw.LineEndCaps;
                Draw.LineEndCaps = LineEndCap.None;

                Camera mainCam = Camera.main;
                Color debugColor = Color.green;
                Color debugColorTransparent = Color.green;
                debugColorTransparent.a = 0.1f;
                Vector3 r = mainCam.transform.right;
                Vector3 u = mainCam.transform.up;

                // Content circumcircle
                Draw.Ring(Vector3.zero, ContentAreaRadius, debugColor);

                // Square screen trimmings
                // TODO: Support MaxScreenSizeForContentScaling
                if (mainCam.pixelWidth != mainCam.pixelHeight || SmallestScreenDimension > MaxScreenSizeForContentScaling)
                {
                    Vector3 rectCornerPosition;
                    if (IsScreenWidthDominant)
                    {
                        rectCornerPosition = new Vector3(ContentAreaRadius * (1f + ContentAreaPadding), -mainCam.orthographicSize, 0);
                    }
                    else
                    {
                        rectCornerPosition = new Vector3(-mainCam.orthographicSize * mainCam.aspect, ContentAreaRadius * (1f + ContentAreaPadding), 0);
                    }

                    float trimFraction = IsScreenWidthDominant ? (float)(mainCam.pixelWidth - mainCam.pixelHeight) / mainCam.pixelWidth : (float)(mainCam.pixelHeight - mainCam.pixelWidth) / mainCam.pixelHeight;
                    float trimLength;
                    if (IsScreenWidthDominant)
                        trimLength = mainCam.aspect * mainCam.orthographicSize * trimFraction;
                    else
                        trimLength = mainCam.orthographicSize * trimFraction;

                    Vector3 trimingBorderSpan = 2f * rectCornerPosition;
                    Vector2 rectSize = rectCornerPosition;
                    if (IsScreenWidthDominant)
                        rectSize = new Vector2(trimLength, -2 * rectSize.y);
                    else
                        rectSize = new Vector2(-2 * rectSize.x, trimLength);

                    Draw.Rectangle(
                        (IsScreenWidthDominant ? -r : r) * rectCornerPosition.x + u * rectCornerPosition.y,
                        mainCam.transform.forward,
                        new Vector2(IsScreenWidthDominant ? (-rectSize.x) : rectSize.x, rectSize.y),
                        RectPivot.Corner,
                        debugColorTransparent);
                    Draw.Rectangle(
                        r * rectCornerPosition.x + (IsScreenWidthDominant ? u : -u) * rectCornerPosition.y,
                        mainCam.transform.forward,
                        new Vector2(rectSize.x, IsScreenWidthDominant ? rectSize.y : (-rectSize.y)),
                        RectPivot.Corner,
                        debugColorTransparent);

                    Draw.Line(
                        (IsScreenWidthDominant ? r : -r) * rectCornerPosition.x + (IsScreenWidthDominant ? -u : u) * trimingBorderSpan.y / 2f,
                        r * rectCornerPosition.x + u * trimingBorderSpan.y / 2f,
                        debugColor);
                    Draw.Line(
                        -r * rectCornerPosition.x - u * trimingBorderSpan.y / 2f,
                        (IsScreenWidthDominant ? -r : r) * rectCornerPosition.x + (IsScreenWidthDominant ? u : -u) * trimingBorderSpan.y / 2f,
                        debugColor);
                }


                // If smallest screen dimension is larger than MaxScreenSizeForContentScaling, draw two more rects to cover what's being trimmed off in the non-dominant dimensions

                // Title menu location
                Draw.Line(
                    u * ContentAreaRadius,
                    u * (IsScreenWidthDominant ? mainCam.orthographicSize : mainCam.orthographicSize * mainCam.aspect),
                    debugColor);
                float paddingLength;
                if (IsScreenWidthDominant)
                    paddingLength = mainCam.orthographicSize - ContentAreaRadius;
                else
                    paddingLength = mainCam.orthographicSize * mainCam.aspect - ContentAreaRadius;
                Draw.Line(
                    u * (ContentAreaRadius + titleMenuPaddingOffset * paddingLength) - r * ContentAreaRadius / 10f,
                    u * (ContentAreaRadius + titleMenuPaddingOffset * paddingLength) + r * ContentAreaRadius / 10f,
                    debugColor);

                // Resetting draw parameters
                Draw.Thickness = previousThickness;
                Draw.LineEndCaps = previousLineEndCap;
            }
        }
    }

    public void RecalculateCoordinateSystem()
    {
        SetGridCorners();
        RecalculateCameraSize();
        SetTitleMenuPosition();
        SetSideMenuPosition();
        // TODO: On play, this throws a null error. No big deal, but annoying. Get rid of it.
        StartCoroutine(StartRecreatingMarkers());
    }

    public void SetGridCorners()
    {
        gridCorners = new List<Vector3>();
        Vector3 refVec = ((Vector3)bounds) / 2f;

        /*
         * Close face:  -z
         * Right face:  +x
         * Top face:    +y
         * 
         *    [5]-----[4]
         *    /|      /|
         *   / |     / |
         * [1]-+---[0] |
         *  | [6]---+-[7]
         *  | /     | /
         *  |/      |/
         * [2]-----[3]
         */

        if (Is2D)
        {
            gridCorners.Add(new Vector3(refVec.x, refVec.y, 0f));
            gridCorners.Add(new Vector3(-refVec.x, refVec.y, 0f));
            gridCorners.Add(new Vector3(-refVec.x, -refVec.y, 0f));
            gridCorners.Add(new Vector3(refVec.x, -refVec.y, 0f));
        }
        else
        {
            gridCorners.Add(new Vector3(refVec.x, refVec.y, -refVec.z));
            gridCorners.Add(new Vector3(-refVec.x, refVec.y, -refVec.z));
            gridCorners.Add(new Vector3(-refVec.x, -refVec.y, -refVec.z));
            gridCorners.Add(new Vector3(refVec.x, -refVec.y, -refVec.z));
            gridCorners.Add(new Vector3(refVec.x, refVec.y, refVec.z));
            gridCorners.Add(new Vector3(-refVec.x, refVec.y, refVec.z));
            gridCorners.Add(new Vector3(-refVec.x, -refVec.y, refVec.z));
            gridCorners.Add(new Vector3(refVec.x, -refVec.y, refVec.z));
        }
    }

    #region Coordinate markers
    public IEnumerator StartRecreatingMarkers()
    {
        InitializeCameraSizes();    // TODO: Consider if needed

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
        int tickCountHorizontal = CalculateTickCount(subdivisions, bounds.x);
        int tickCountVertical = CalculateTickCount(subdivisions, bounds.y);
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

    // TODO: Prevent from overwhelming system on large content areas
    private void CreateTickMarkerChildren(int tickCount, Direction side, Transform tickParent)
    {
        for (int i = 0; i < tickCount; i++)
        {
            float linearProgress = (float)i / (tickCount - 1);
            GameObject child = CreateMarkerChild(lineObject, tickParent);
            Vector3Pair cornerPoints = SelectGridCorners(side);
            Vector3 position = cornerPoints.A + linearProgress * (cornerPoints.B - cornerPoints.A);
            int skewTickPatternOffset = halfSkewTickPattern ? ((tickCount - 1) / 2) : 0;
            int sideSize = (side == Direction.East || side == Direction.West) ? bounds.x : bounds.y;
            float magnitude = CalculateTickLength(tickMarkerLength.x, tickMarkerLength.y, i, subdivisions, sideSize, skewTickPatternOffset);
            bool isLateral = side == Direction.East || side == Direction.West;
            if (relativeMarkerLength)
            {
                magnitude = magnitude / (isLateral ? bounds.x : bounds.y);
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

    // TODO: Prevent from overwhelming system on large content areas
    private void CreateGridMarkerChildren(MarkerType type, Transform gridParent)
    {
        // TODO: Implement 3D
        // Corner points, starting from north-east, going counter-clockwise
        Vector3 NE = gridCorners[0];
        Vector3 NW = gridCorners[1];
        Vector3 SW = gridCorners[2];
        Vector3 SE = gridCorners[3];
        Vector3 rightInterval = SE - SW;
        Vector3 rightNormalized = rightInterval.normalized;
        Vector3 upInterval = NW - SW;
        Vector3 upNormalized = upInterval.normalized;
        int tickCountUnitInterval = CalculateTickCount(subdivisions, bounds.x);
        int tickCountX = CalculateTickCount(subdivisions, bounds.x);
        int tickCountY = CalculateTickCount(subdivisions, bounds.y);
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
                childHorizontal.name = "Gridline (x = " + (linearProgress * bounds.x * 2 - bounds.x).ToString() + ")";
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
                childVertical.name = "Gridline (y = " + (linearProgress * bounds.y * 2 - bounds.y).ToString() + ")";
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
                    child.name = childName + " (x = " + (linearProgressX * bounds.x * 2 - bounds.x).ToString() + ", y = " + (linearProgressY * bounds.y * 2 - bounds.y) + ")";
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

    // TODO: Enable for 3D
    private Vector3Pair SelectGridCorners(Direction side)
    {
        Vector3Pair points = new Vector3Pair(Vector3.zero, Vector3.zero);

        switch (side)
        {
            case Direction.East:
            {
                points.A = gridCorners[0];
                points.B = gridCorners[3];
                break;
            }
            case Direction.North:
            {
                points.A = gridCorners[1];
                points.B = gridCorners[0];
                break;
            }
            case Direction.West:
            {
                points.A = gridCorners[2];
                points.B = gridCorners[1];
                break;
            }
            case Direction.South:
            {
                points.A = gridCorners[3];
                points.B = gridCorners[2];
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
    #endregion // Coordinate markers

    // TODO: Set above furthest diagonal, not just topmost point when at default orientation
    private void SetTitleMenuPosition()
    {
        //float screenSizeLimit = Camera.main.pixelHeight > MaxScreenSizeForContentScaling ? MaxScreenSizeForContentScaling : Camera.main.pixelHeight;
        //float yOffset = titleMenuPaddingRelative ? (titleMenuPaddingOffset * screenSizeLimit / 2f) : titleMenuPaddingOffset;
        //float yPosition = Camera.main.WorldToScreenPoint(gridCorners[0]).y - Camera.main.pixelHeight / 2f + yOffset;

        //titleMenuPositionCached = new Vector3(0f, yPosition, 0f);

        //titleMenu.GetComponent<RectTransform>().localPosition = titleMenuPositionCached;

        Vector3 titleMenuWorldPosition = new Vector3(0f, 0.5f * ContentAreaRadius * (1f + ContentAreaPadding), 0f);
        titleMenuPositionCached = Camera.main.WorldToScreenPoint(titleMenuWorldPosition);
        titleMenu.GetComponent<RectTransform>().localPosition = titleMenuPositionCached;
    }

    private void SetSideMenuPosition()
    {
        Vector3 newSidePosition = new Vector3(SideMenuOffsetPixels.x, SideMenuOffsetPixels.y, 0f);
        // Assuming the title is perfectly centered on the x-axis,
        // checks if overflowing the screen on the right side
        Rect menuRect = sideMenu.GetComponent<RectTransform>().rect;
        if (SideMenuOffsetPixels.x + menuRect.width > Camera.main.pixelWidth / 2f)
        {
            newSidePosition.x = Camera.main.pixelWidth / 2f - menuRect.width;
        }
        // Checks if overflowing the screen at the bottom (y is a free variable)
        if (titleMenuPositionCached.y + SideMenuOffsetPixels.y - menuRect.height < -Camera.main.pixelHeight / 2f)
        {
            newSidePosition.y = -titleMenuPositionCached.y - Camera.main.pixelHeight / 2f + menuRect.height;
        }
        sideMenu.GetComponent<RectTransform>().localPosition = newSidePosition;
    }

    public void RecalculateCameraSize()
    {
        // 1. Trim screen rectangle and operate only on largest screen square
        smallestScreenDimension = IsScreenWidthDominant ? cam.pixelHeight : cam.pixelWidth;

        // 2. Inscribe content area cuboid inside a sphere
        float cameraSize = GridCorners[0].magnitude;

        // 3. Add padding on top of sphere's radius
        cameraSize *= (1f + ContentAreaPadding);

        // 4. Cap content sphere after set screen size
        if (SmallestScreenDimension > MaxScreenSizeForContentScaling)
        {
            cameraSize *= (float)SmallestScreenDimension / MaxScreenSizeForContentScaling;
        }

        // 5. Set title on top of current content sphere radius, not attached to edge of screen

        // orthographicSize only counts vertical half-height
        cam.orthographicSize = IsScreenWidthDominant ? cameraSize : cameraSize / cam.aspect;
    }

    private bool WasCameraResized()
    {
        return (currentCameraWidth != previousCameraWidth || currentCameraHeight != previousCameraHeight);
    }

    private bool WasContentAreaResized()
    {
        return previousBounds != bounds;
    }

    private void InitializeCameraSizes()
    {
        currentCameraWidth = cam.pixelWidth;
        currentCameraHeight = cam.pixelHeight;
        previousCameraWidth = currentCameraWidth;
        previousCameraHeight = currentCameraHeight;
    }
}
