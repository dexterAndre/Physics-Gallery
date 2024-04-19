using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using System;
using static BehaviorSpecifications;



/*
    TODO:
    [X] Delete old files
    [X] Rename new files
    [X] Rearchitect adding of components and synchronization of them, also how references are gotten at construction
    [X] Implement singleton class for easier lookup (try to somehow get ManagerGUI and ManagerPointSet as Singletons while enabling drawing of shapes)
    [ ] Presets
    [ ] Broken: Wrap functions
    [X] Broken: Jitter relative speed
    [X] Broken: Add component dropdown (shifted by +1?)
    [ ] Animate -> Animation, Selector -> Selection
    [X] Rearrange buttons
    [ ] Add a few more options and functional adding / removing of components
    [ ] Disable edge organization buttons, as they won't do anything anyway
    [ ] Warning when 2D / 3D is disabled (with clear inticator for the reason)
    [X] Obsoletify Component Function Parent
    [X] Collapsible components
    [X] Enable / disable component (gray out, disable interaction)
    [X] Auto-populate dropdowns
    [X] Animation Jitter component
    [X] Add Component dropdown
    [X] Colorables
    [X] Delete component
    [ ] Ensure scrolling works
    [X] Collabsible interactable area entire header background
    [ ] Get rid of scrollbar space when not in use. Or get rid of it completely?
    [ ] Colorable scroll bar
    [X] Prevent camera movement while interacting with GUI (simple disable OnMouseOver?)
    [X] Rename to ManagerPointSet and ManagerGUI
    [ ] Points do not disappear (2D, overflow, but probably unrelated as I suspect this is a deeper issue)
    [X] Switching from 3D to 2D leaves front face, not a centered face in the coordinate system bounds
    [ ] Support multiple components of same type
*/

#region Structs and enums
[System.Serializable]
public enum PrefabType
{
    Plus,
    Cross,
    Disc,
    Circle,
    Triangle,
    Boid,
}
[System.Serializable]
public struct PosRot2
{
    public PosRot2(Vector2 inPosition)
    {
        position = inPosition;
        rotation = Vector2.up;
    }
    public PosRot2(Vector2 inPosition, Vector2 inRotation)
    {
        position = inPosition;
        rotation = inRotation;
    }

    public Vector2 position;
    public Vector2 rotation;
}
[System.Serializable]
public struct PosRot3
{
    public PosRot3(Vector3 inPosition)
    {
        position = inPosition;
        rotation = Vector3.forward;
    }
    public PosRot3(Vector3 inPosition, Vector3 inRotation)
    {
        position = inPosition;
        rotation = inRotation;
    }

    public Vector3 position;
    public Vector3 rotation;
}
public class PointAnimProxy
{
    // Returns whether the animation has finished or is ongoing
    public bool UpdateBehavior(float deltaTime)
    {
        animationTimer += deltaTime;
        return IsAnimationFinished();
    }

    public float GetAnimationProgress()
    {
        return animationTimer / animationDuration;
    }

    public bool IsAnimationFinished()
    {
        return animationTimer >= animationDuration;
    }

    public float animationDuration;
    public float animationTimer;
}
[System.Serializable]
public class PointAnimProxy3 : PointAnimProxy
{
    public PointAnimProxy3(Vector3 inPosition = default(Vector3), float inAnimationDuration = 1f)
    {
        position = inPosition;
        animationDuration = inAnimationDuration;
        animationTimer = 0f;
    }
    public PointAnimProxy3(Vector3 inPosition = default(Vector3), float inAnimationDuration = 1f, float inAnimationTimer = 0f)
    {
        position = inPosition;
        animationDuration = inAnimationDuration;
        animationTimer = inAnimationTimer;
    }

    public Vector3 position;
}
#endregion // Structs and enums

public class Manager_PointSet : ImmediateModeShapeDrawer
{
    // Points
    [SerializeField] private List<Vector3> positions3 = new List<Vector3>();
    //[SerializeField] private List<PosRot3> posrots3 = new List<PosRot3>();    // TODO: For use with Boids etc.
    [SerializeField] private List<PointAnimProxy3> wrapProxies3 = new List<PointAnimProxy3>();
    public void UpdateDimension(bool InIs2D)
    {
        bool settingsIs2D = Manager_Lookup.Instance.ManagerGUI.ComponentSettings.Is2D;

        if (InIs2D == settingsIs2D)
            return;

        // Updating grid points
        guiCoordinateSystem.RecalculateCoordinateSystem();

        // Setting delegates
        Generate_MasterFunction = settingsIs2D ? GeneratePoints_Random2D : GeneratePoints_Random3D;
        WrapPoint_MasterFunction = settingsIs2D ? WrapPoint_Rectangular2D : WrapPoint_Rectangular3D;
    }

    [Header("Spawning")]
    [SerializeField] private Transform pointsParent;
    [SerializeField] private PrefabType pointType = PrefabType.Circle;
    private PrefabType previousPointType = PrefabType.Circle;
    [SerializeField] private GUIOption_IncrementalSlider guiPointCount;
    [SerializeField] private GUIOption_DropdownGallery guiGenerationMethod;
    [SerializeField] private CoordinateSystem guiCoordinateSystem;
    [SerializeField] private GUIComponent_Settings settings;

    [Header("Points")]
    [SerializeField, Range(0f, 0.05f)] private float pointSize = 0.001f;
    [SerializeField] private LineGeometry lineGeometry = LineGeometry.Billboard;
    [SerializeField] private DiscGeometry discGeometry = DiscGeometry.Billboard;
    [SerializeField] private ThicknessSpace thicknessSpace = ThicknessSpace.Pixels;
    [SerializeField] private float thickness = 3;
    private Matrix4x4 drawMatrix;
    // These shape coordinates are initialized, normalized, and used to read from in subsequent draw calls
    [SerializeField] Transform pointPrefabsParent;
    private List<Vector3> shapeCoordinates_plus;
    private List<Vector3> shapeCoordinates_cross;
    private List<Vector3> shapeCoordinates_triangle;
    private List<Vector3> shapeCoordinates_boid;

    [Header("Wrapping")]
    [SerializeField] private float wrapAnimationDuration = 1f;
    [SerializeField] private float wrapAnimationRadiusMultiplier = 1f;
    [SerializeField] private float wrapAnimationThickness = 3f;

    // Misc.
    public Vector3Int Bounds { get { return guiCoordinateSystem.bounds; } }

    //[Header("Overlays")]
    //[SerializeField] private float overlayThickness = 0.25f;

    // Callbacks
    public delegate void VoidDelegate_ZeroParameters();
    public delegate void VoidDelegate_Int(int arg1);
    public delegate void VoidDelegate_Vector3Float(Vector3 arg1, float arg2);

    // Generation delegates
    public static event VoidDelegate_ZeroParameters Generate_MasterFunction;

    // Misc. delegates
    public static event VoidDelegate_Int WrapPoint_MasterFunction;
    public static event VoidDelegate_Vector3Float DrawPoint_MasterFunction;



    // Animation components
    // TODO: Rename to behaviorsAnimation, behaviorsOverlay, behaviorsSelection
    [SerializeField] private List<PointBehavior> animationBehaviors = new List<PointBehavior>();
    public List<PointBehavior> AnimationBehaviors { get { return animationBehaviors; } }
    [SerializeField] private List<PointBehavior> overlayBehaviors = new List<PointBehavior>();
    public List<PointBehavior> OverlayBehaviors { get { return overlayBehaviors; } }
    [SerializeField] private List<PointBehavior> selectionBehaviors = new List<PointBehavior>();
    public List<PointBehavior> SelectionBehaviors { get { return selectionBehaviors; } }
    //[SerializeField] private List<PointBehavior_Animate> overlayBehaviors = new List<PointBehavior_Animate>();
    //[SerializeField] private List<PointBehavior_Animate> selectionBehaviors = new List<PointBehavior_Animate>();
    [SerializeField]
    //private Dictionary<EdgeResponse, Action<int>> edgeResponseMethods = new Dictionary<EdgeResponse, Action<int>>();
    private Dictionary<EdgeResponse, VoidDelegate_Int> edgeResponseMethods = new Dictionary<EdgeResponse, VoidDelegate_Int>();
    public Dictionary<EdgeResponse, VoidDelegate_Int> EdgeResponseMethods { get { return edgeResponseMethods; } }
    [SerializeField] private Dictionary<GenerationMethod, Action> generationMethods = new Dictionary<GenerationMethod, Action>();
    public Dictionary<GenerationMethod, Action> GenerationMethods { get { return generationMethods; } }



    private void Awake()
    {
        UpdateDrawParameters();
        EvaluateShapeCoordinates();
        SetPointType(pointType);    // Runs setter, which selects drawing delegate
        animationBehaviors.Clear();

        // GUI-related reading
        StartCoroutine(ReadGUIData());
    }

    public override void OnEnable()
    {
        base.OnEnable();

        previousPointType = pointType;

        Generate_MasterFunction = GeneratePoints_Random3D;
        // TODO: Make wrapping functions static, passing in the list and index instead
        //WrapPoint2D_MasterFunction = WrapPoint_Rectangular3D;
        WrapPoint_MasterFunction = WrapPoint_Rectangular3D;

        CollectEdgeResponseMethods();
        
        drawMatrix = transform.localToWorldMatrix;
        UpdateDrawParameters();
        EvaluateShapeCoordinates();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        Generate_MasterFunction = null;
        WrapPoint_MasterFunction = null;
    }

    private void OnValidate()
    {
        UpdateDrawParameters();
        CollectEdgeResponseMethods();

        if (previousPointType != pointType)
        {
            previousPointType = pointType;
            SetPointType(pointType);
        }
    }

    // Behaviors
    private void Update()
    {
        // 1. For all behaviors, enable / disable (with warning over the options area) based on dimensionality where appropriate

        // 2. For all behaviors, warn if framerate will be lower than X FPS

        // 3. Update all animation behaviors
        // TODO: Split off 2D behaviors and 3D on a delegate
        foreach (PointBehavior behavior in animationBehaviors)
        {
            for (int i = 0; i < positions3.Count; i++)
            {
                MovePoint(i, behavior.UpdateBehavior(positions3, i) * Time.deltaTime);
            }
        }

        // 4. Process edge responses after all movement has been made
        if (WrapPoint_MasterFunction != null)
        {
            for (int i = 0; i < positions3.Count; i++)
            {
                WrapPoint_MasterFunction.Invoke(i);
            }
        }

        // 5. Update all overlay behaviors

        // 5.1 Initialize on first frame

        // 6. Update all selector behaviors



        //if (is2D)
        //{
        //    for (int i = 0; i < positions2.Count; i++)
        //    {
        //        Vector2 movement = Vector2.zero;
        //        Vector2 position = positions2[i];
        //        foreach (PointBehavior_Animate animationBehavior in animationBehaviors)
        //        {
        //            movement += animationBehavior.UpdateBehavior(position);
        //        }

        //        WrapPoint2D_MasterFunction?.Invoke(i, movement);
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < positions3.Count; i++)
        //    {
        //        Vector3 movement = Vector3.zero;
        //        Vector3 position = positions3[i];
        //        foreach (PointBehavior_Animate animationBehavior in animationBehaviors)
        //        {
        //            movement += animationBehavior.UpdateBehavior(position);
        //        }

        //        WrapPoint_MasterFunction?.Invoke(i, movement);
        //    }
        //}

        // Updating overlay behaviors
        //  ...

        // Updating death proxies
        // TODO: Lock updates when switching dimension?
        //if (is2D)
        //{
        //    for (int i = wrapProxies2.Count - 1; i >= 0; i--)
        //    {
        //        PointAnimProxy2 proxy = wrapProxies2[i];
        //        proxy.animationTimer += Time.deltaTime;

        //        if (proxy.animationTimer > proxy.animationDuration)
        //        {
        //            wrapProxies2.RemoveAt(i);
        //        }
        //        else
        //        {
        //            wrapProxies2[i] = proxy;
        //        }
        //    }
        //}
        //else
        //{
            for (int i = wrapProxies3.Count - 1; i >= 0; i--)
            {
                PointAnimProxy3 proxy = wrapProxies3[i];
                proxy.animationTimer += Time.deltaTime;

                if (proxy.animationTimer > proxy.animationDuration)
                {
                    wrapProxies3.RemoveAt(i);
                }
                else
                {
                    wrapProxies3[i] = proxy;
                }
            }
        //}
    }

    private IEnumerator ReadGUIData()
    {
        yield return new WaitForEndOfFrame();
        UpdateDimension(settings.Is2D);
        yield return null;
    }

    #region Behavior functionality
    #region Components
    public void AddBehavior(PointBehavior behavior)
    {
        if (behavior == null)
        {
            Debug.LogWarning("Attempted adding null behavior. Aborting operation.");
            return;
        }

        switch (behavior.Type)
        {
            case BehaviorType.Animation:
                {
                    animationBehaviors.Add(behavior);
                    break;
                }
            case BehaviorType.Overlay:
                {
                    overlayBehaviors.Add(behavior);
                    break;
                }
            case BehaviorType.Selection:
                {
                    selectionBehaviors.Add(behavior);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    public void RemoveBehavior(PointBehavior behavior)
    {
        if (behavior == null)
        {
            Debug.LogWarning("Attempted removing null behavior. Aborting operation.");
            return;
        }

        switch (behavior.Type)
        {
            case BehaviorType.Animation:
            {
                animationBehaviors.Remove(behavior);
                break;
            }
            case BehaviorType.Overlay:
            {
                overlayBehaviors.Remove(behavior);
                break;
            }
            case BehaviorType.Selection:
            {
                selectionBehaviors.Remove(behavior);
                break;
            }
            default:
            {
                break;
            }
        }
    }
    #endregion // Components
    #region Generation
    public void ClearPoints()
    {
        //positions2.Clear();
        positions3.Clear();

        // TODO: Clear wrap proxies too (but make sure they aren't currently being read from)
    }
    public void Generate()
    {
        ClearPoints();
        Generate_MasterFunction();
    }
    private Vector2 GenerateRandomPoint2D_Rectangular(Vector2 size)
    {
        return new Vector2(
            UnityEngine.Random.Range(-size.x, size.x),
            UnityEngine.Random.Range(-size.y, size.y));
    }
    private Vector3 GenerateRandomPoint3D_Rectangular(Vector3 size)
    {
        return new Vector3(
            UnityEngine.Random.Range(-size.x, size.x),
            UnityEngine.Random.Range(-size.y, size.y),
            UnityEngine.Random.Range(-size.z, size.z));
    }
    //private Vector2 GenerateRandomPoint_Circle(float radius)
    //{
    //    return Vector2.zero;
    //}
    //private Vector2 GenerateRandomPoint_Sector(float angle, float radius)
    //{
    //    return Vector2.zero;
    //}
    public void GeneratePoints_Random2D()
    {
        if (guiPointCount == null)
        {
            Debug.LogError("Could not find GUIOption_IncrementalSliderInput \"GUIPointCount\". Could not read point count when attempting to generate points. Failed operation.");
            return;
        }

        if (guiCoordinateSystem == null)
        {
            Debug.LogError("CoordinateSystem is null! Could not read bounds.");
            return;
        }

        Vector2 bounds = (Vector2Int)guiCoordinateSystem.bounds;
        bounds /= 2f;

        // TODO: Switch on bounds shape
        for (int i = 0; i < guiPointCount.IncrementalSlider.SliderValue; i++)
        {
            positions3.Add(GenerateRandomPoint2D_Rectangular(bounds));
        }
    }
    public void GeneratePoints_Random3D()
    {
        if (guiPointCount == null)
        {
            Debug.LogError("GUIIncrementSliderInput is null! Could not read point count.");
            return;
        }

        if (guiCoordinateSystem == null)
        {
            Debug.LogError("CoordinateSystem is null! Could not read bounds.");
            return;
        }

        Vector3 bounds = (Vector3Int)guiCoordinateSystem.bounds;
        bounds /= 2f;
        
        // TODO: Switch on bounds shape
        for (int i = 0; i < guiPointCount.IncrementalSlider.SliderValue; i++)
        {
            positions3.Add(GenerateRandomPoint3D_Rectangular(bounds));
        }
    }
    public void GeneratePoints_BlueNoise2D()
    {
        throw new NotImplementedException();
    }
    public void GeneratePoints_BlueNoise3D()
    {
        throw new NotSupportedException();
    }
    public void GeneratePoints_LatticeRectangular2D()
    {
        throw new NotImplementedException();
    }
    public void GeneratePoints_LatticeRectangular3D()
    {
        throw new NotImplementedException();
    }
    public void GeneratePoints_LatticeHexagonal2D()
    {
        throw new NotImplementedException();
    }
    // TODO: Is this even possible?
    //public void GeneratePoints_LatticeHexagonal3D()
    //{
    //    throw new NotImplementedException();
    //}
    public void GeneratePoints_DoubleSlitDistribution()
    {
        throw new NotImplementedException();
    }
    public void GeneratePoints_GaussianDistribution2D()
    {
        throw new NotImplementedException();
    }
    public void GeneratePoints_GaussianDistribution3D()
    {
        throw new NotImplementedException();
    }
    // TODO: May need to force-swap between dimensions. Possibly a toast popup message?
    public void GeneratePoints_Import()
    {
        throw new NotImplementedException();
    }
    #endregion  // Generation
    #region Overlay
    #endregion  // Overlay
    #region Select
    #endregion  // Select
    #region Animation
    #endregion  // Animation
    #endregion  // Behavior functionality

    #region Points
    public void SetPoint(int index, Vector3 position)
    {
        positions3[index] = position;
    }
    public void MovePoint(int index, Vector3 displacement)
    {
        positions3[index] += (displacement * Time.deltaTime);
    }
    public void WrapPoint_Rectangular2D(int index)
    {
        Vector3 wrapAnimationPosition = Vector3.zero;
        Vector3 boundsHalf_Cached = guiCoordinateSystem.BoundsHalf;
        Vector3 point = positions3[index];
        if (!MathUtils.IsInside_AABB_2D(point, boundsHalf_Cached, ref wrapAnimationPosition))
        {
            // Wraps point
            SetPoint(index, MathUtils.Wrap(point, -boundsHalf_Cached, boundsHalf_Cached));
            // Leaves tombstone indicator that point has wrapped
            wrapProxies3.Add(new PointAnimProxy3(wrapAnimationPosition, wrapAnimationDuration));
        }
    }
    public void WrapPoint_Rectangular3D(int index)
    {
        Vector3 wrapAnimationPosition = Vector3.zero;
        Vector3 boundsHalf_Cached = guiCoordinateSystem.BoundsHalf;
        Vector3 point = positions3[index];
        if (!MathUtils.IsInside_AABB_3D(point, boundsHalf_Cached, ref wrapAnimationPosition))
        {
            // Wraps point
            SetPoint(index, MathUtils.Wrap(point, -boundsHalf_Cached, boundsHalf_Cached));
            // Leaves tombstone indicator that point has wrapped
            wrapProxies3.Add(new PointAnimProxy3(wrapAnimationPosition, wrapAnimationDuration));
        }
    }
    public int PointCount()
    {
        return positions3.Count;
    }
    #endregion // Points

    #region Drawing
    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            // Drawing points
            //if (is2D)
            //{
            //    foreach (Vector2 pos in positions2)
            //    {
            //        DrawPoint_MasterFunction?.Invoke(pos, pointSize);
            //    }
            //}
            //else
            //{
                foreach (Vector3 pos in positions3)
                {
                    DrawPoint_MasterFunction?.Invoke(pos, pointSize);
                }
            //}

            // Drawing overlays
            //      n^2 web
            // TODO: Replace with component-based drawing
            //Draw.Thickness = overlayThickness;
            //for (int i = 0; i < positions3.Count - 1; i++)
            //{
            //    for (int j = i; j < positions3.Count; j++)
            //    {
            //        Draw.Line(positions3[i], positions3[j]);
            //    }
            //}

            // Drawing wrap event indicator animations
            Draw.Thickness = wrapAnimationThickness;
            //if (is2D)
            //{
            //    foreach (PointAnimProxy2 wrapProxy in wrapProxies2)
            //    {
            //        Draw.Ring(wrapProxy.position, pointSize * wrapAnimationRadiusMultiplier * wrapProxy.GetAnimationProgress());
            //    }
            //}
            //else
            //{
                foreach (PointAnimProxy3 wrapProxy in wrapProxies3)
                {
                    Draw.Ring(wrapProxy.position, pointSize * wrapAnimationRadiusMultiplier * wrapProxy.GetAnimationProgress());
                }
            //}
        }
    }

    private void DrawPoint_Plus(Vector3 position, float radius)
    {
        Draw.Line(position + shapeCoordinates_plus[0], position + shapeCoordinates_plus[1]);
        Draw.Line(position + shapeCoordinates_plus[2], position + shapeCoordinates_plus[3]);
    }
    private void DrawPoint_Cross(Vector3 position, float radius)
    {
        Draw.Line(position + shapeCoordinates_cross[0], position + shapeCoordinates_cross[1]);
        Draw.Line(position + shapeCoordinates_cross[2], position + shapeCoordinates_cross[3]);
    }
    private void DrawPoint_Disc(Vector3 position, float radius)
    {
        Draw.Disc(position, pointSize);
    }
    private void DrawPoint_Circle(Vector3 position, float radius)
    {
        Draw.Ring(position, pointSize);
    }
    private void DrawPoint_Triangle(Vector3 position, float radius)
    {

    }
    private void DrawPoint_Boid(Vector3 position, float radius)
    {

    }

    private void UpdateDrawParameters()
    {
        drawMatrix = transform.localToWorldMatrix;
        Draw.LineGeometry = lineGeometry;
        Draw.DiscGeometry = discGeometry;
        Draw.ThicknessSpace = thicknessSpace;
        Draw.Thickness = thickness;
        Draw.Matrix = drawMatrix;
    }

    private void EvaluateShapeCoordinates()
    {
        Line plusLineHorizontal = pointPrefabsParent.Find("Plus").Find("Horizontal Stroke").GetComponent<Line>();
        Line plusLineVertical = pointPrefabsParent.Find("Plus").Find("Vertical Stroke").GetComponent<Line>();
        shapeCoordinates_plus = new List<Vector3>
        {
            plusLineHorizontal.Start.normalized * pointSize,
            plusLineHorizontal.End.normalized * pointSize,
            plusLineVertical.Start.normalized * pointSize,
            plusLineVertical.End.normalized * pointSize
        };

        Line crossLineNE = pointPrefabsParent.Find("Cross").Find("NE Stroke").GetComponent<Line>();
        Line crossLineSE = pointPrefabsParent.Find("Cross").Find("SE Stroke").GetComponent<Line>();
        shapeCoordinates_cross = new List<Vector3>
        {
            crossLineNE.Start.normalized * pointSize,
            crossLineNE.End.normalized * pointSize,
            crossLineSE.Start.normalized * pointSize,
            crossLineSE.End.normalized * pointSize
        };

        Triangle triangle = pointPrefabsParent.Find("Triangle").GetComponent<Triangle>();
        shapeCoordinates_triangle = new List<Vector3>
        {
            triangle.A.normalized * pointSize,
            triangle.B.normalized * pointSize,
            triangle.C.normalized * pointSize
        };

        Quad boid = pointPrefabsParent.Find("Boid").GetComponent<Quad>();
        shapeCoordinates_boid = new List<Vector3>
        {
            boid.A.normalized * pointSize,
            boid.B.normalized * pointSize,
            boid.C.normalized * pointSize,
            boid.D.normalized * pointSize
        };
    }
    #endregion // Drawing

    #region Member variables
    private void CollectEdgeResponseMethods()
    {
        // Would not initialize at declaration, so I'm manually collecting them here. 
        // This is always called from OnEnable.
        edgeResponseMethods.Clear();
        edgeResponseMethods = new Dictionary<EdgeResponse, VoidDelegate_Int>
        {
            { EdgeResponse.Overflow,    null },
            { EdgeResponse.Wrap,        WrapPoint_Rectangular3D },
        };
    }

    public void SetEdgeResponse(EdgeResponse response)
    {
        WrapPoint_MasterFunction = EdgeResponseMethods[response];
    }
    public PrefabType SetPointType(PrefabType type)
    {
        pointType = type;
        switch (pointType)
        {
            case PrefabType.Plus:
            {
                DrawPoint_MasterFunction = DrawPoint_Plus;
                break;
            }
            case PrefabType.Cross:
            {
                DrawPoint_MasterFunction = DrawPoint_Cross;
                break;
            }
            case PrefabType.Disc:
            {
                DrawPoint_MasterFunction = DrawPoint_Disc;
                break;
            }
            case PrefabType.Circle:
            {
                DrawPoint_MasterFunction = DrawPoint_Circle;
                break;
            }
            case PrefabType.Triangle:
            {
                DrawPoint_MasterFunction = DrawPoint_Triangle;
                break;
            }
            case PrefabType.Boid:
            {
                DrawPoint_MasterFunction = DrawPoint_Boid;
                break;
            }
            default:
            {
                DrawPoint_MasterFunction = null;
                break;
            }
        }

        return pointType;
    }
    #endregion // Member variables
}
