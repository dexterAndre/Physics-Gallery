using JetBrains.Annotations;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.UI;
using UnityEngine.UIElements;

/**
    To do:
    - Graphic for when point dies
    - Add components via menu (dropdown?)
    - Remove components via button
    - Reorder components via button
    - 3D
    - Camera control 3D (permanent rotate, rotate and snapback)
*/



[System.Serializable]
public enum PrefabType
{
    Plus,
    Cross,
    Disc,
    Circle,
    Triangle,
    Boid,
    SquareFilled,
    SquareHollow
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
public class PointAnimProxy2 : PointAnimProxy
{
    public PointAnimProxy2(Vector2 inPosition = default(Vector2), float inAnimationDuration = 1f)
    {
        position = inPosition;
        animationDuration = inAnimationDuration;
        animationTimer = 0f;
    }

    public Vector2 position;
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

    public Vector3 position;
}
[System.Serializable]
public enum BoundsType
{
    // 2D
    Square,
    Circle,
    Sector,

    // 3D
    Cube,
    Sphere,
    Cone
}
[System.Serializable]
public enum EdgeResponse
{
    Overflow,
    Wrap,
    Kill,
    Respawn
}
[System.Serializable]
public enum GenerationMethod
{
    Random,
    PoissonDisc,
    LatticeRectangular,
    LatticeHexagonal,
    DoubleSlitDistribution,
    GaussianDistribution,
    Import,
};
[System.Serializable]
public enum OverlayMethod
{
    Web,
    Triangulation,
    ConvexHull,
    VoronoiDiagram,
    Duals,
    SpatialPartitioning,
    CenterOfMass,
}
[System.Serializable]
public enum SelectionMethod
{
    ClosestPointToRay,
    kMeansClustering,
    PointSetRegistration,
}
[System.Serializable]
public enum AnimationMethod
{
    Jitter,
    Flocking,
    VectorField,
    WindSimulation,
    StrangeAttractor,
    LotkaVolterraEquations,
    SpringSystem,
}



public class ManagerPointSet : ImmediateModeShapeDrawer
{
    [SerializeField] private List<Vector2> positions2 = new List<Vector2>();
    public void SetPoint(int index, Vector2 point)
    {
        positions2[index] = point;
    }
    public void MovePoint(int index, Vector3 displacement)
    {
        Vector3 newPos = positions3[index] + displacement * Time.deltaTime;
        positions3[index] = newPos;
    }
    public void MovePoint_WrappedRectangular(int index, Vector3 displacement)
    {
        Vector3 newPos = positions3[index] + displacement * Time.deltaTime;
        Vector3 wrapAnimationPosition = newPos;

        // Wrapping
        Vector3 boundsHalf = guiCoordinateSystem.BoundsHalf;
        Vector3 interval = (Vector3)guiCoordinateSystem.Bounds;
        bool shouldWrap = false;
        // TODO: Should consider corner cases, where it may have exeeded the bounds on several axes in one update
        // For now, just project
        if (newPos.x < -boundsHalf.x)
        {
            newPos.x += interval.x;
            shouldWrap = true;
            wrapAnimationPosition = MathUtils.ClosestPointToLine(wrapAnimationPosition, guiCoordinateSystem.GridCorners[2].point, Vector3.up);
        }
        else if (newPos.x > boundsHalf.x)
        {
            newPos.x -= interval.x;
            shouldWrap = true;
            wrapAnimationPosition = MathUtils.ClosestPointToLine(wrapAnimationPosition, guiCoordinateSystem.GridCorners[3].point, Vector3.up);
        }
        if (newPos.y < -boundsHalf.y)
        {
            newPos.y += interval.y;
            shouldWrap = true;
            wrapAnimationPosition = MathUtils.ClosestPointToLine(wrapAnimationPosition, guiCoordinateSystem.GridCorners[2].point, Vector2.right);
        }
        else if (newPos.y > boundsHalf.y)
        {
            newPos.y -= interval.y;
            shouldWrap = true;
            wrapAnimationPosition = MathUtils.ClosestPointToLine(wrapAnimationPosition, guiCoordinateSystem.GridCorners[1].point, Vector2.right);
        }
        // TODO: Complete z-axis wrapping
        if (newPos.z < -boundsHalf.z)
        {
            newPos.z += interval.z;
            shouldWrap = true;
            //wrapAnimationPosition = MathUtils.ClosestPointToLine(wrapAnimationPosition, guiCoordinateSystem.GridCorners[2].point, Vector2.right);
        }
        else if (newPos.z > boundsHalf.z)
        {
            newPos.z -= interval.z;
            shouldWrap = true;
            //wrapAnimationPosition = MathUtils.ClosestPointToLine(wrapAnimationPosition, guiCoordinateSystem.GridCorners[1].point, Vector2.right);
        }

        if (shouldWrap)
        {
            wrapProxies.Add(new PointAnimProxy3(wrapAnimationPosition, wrapAnimationDuration));
        }

        positions3[index] = newPos;
    }
    public int PointCount()
    {
        return positions2.Count;
    }
    [SerializeField] private List<PosRot2> posrots2 = new List<PosRot2>();
    [SerializeField] private List<Vector3> positions3 = new List<Vector3>();
    [SerializeField] private List<PosRot3> posrots3 = new List<PosRot3>();
    [SerializeField] private List<PointAnimProxy3> wrapProxies = new List<PointAnimProxy3>();

    [Header("Spawning")]
    [SerializeField] private Transform pointsParent;
    [SerializeField] private PrefabType pointType = PrefabType.SquareFilled;
    [SerializeField] private GUIIncrementSliderInput guiPointCount;
    [SerializeField] private TMP_Dropdown guiGenerationMethod;
    [SerializeField] private CoordinateOverlayCartesian guiCoordinateSystem;

    [Header("Points")]
    [SerializeField, Range(0f, 1f)] private float pointSize = 0.001f;
    [SerializeField] private LineGeometry lineGeometry = LineGeometry.Billboard;
    [SerializeField] private ThicknessSpace thicknessSpace = ThicknessSpace.Pixels;
    [SerializeField] private float thickness = 3;
    private Matrix4x4 drawMatrix;
    // Normalizing shape coordinates, should only be done to initialize
    [SerializeField] Transform pointPrefabsParent;
    private List<Vector2> shapeCoordinates_plus;
    private List<Vector2> shapeCoordinates_cross;
    private float shapeCoordinates_circle;
    private List<Vector2> shapeCoordinates_triangle;
    private List<Vector2> shapeCoordinates_boid;
    private List<Vector2> shapeCoordinates_square;

    [Header("Wrapping")]
    [SerializeField] private float wrapAnimationDuration = 1f;
    [SerializeField] private float wrapAnimationRadiusMultiplier = 1f;
    [SerializeField] private float wrapAnimationThickness = 3f;
    [SerializeField] private EdgeResponse edgeResponse;
    public void SetEdgeResponse(int enumIndex)
    {
        edgeResponse = (EdgeResponse)enumIndex;
        switch (edgeResponse)
        {
            case EdgeResponse.Overflow:
            {
                WrapPoint_MasterFunction = MovePoint;
                break;
            }
            case EdgeResponse.Wrap:
            {
                WrapPoint_MasterFunction = MovePoint_WrappedRectangular;
                break;
            }
            case EdgeResponse.Kill:
            {
                break;
            }
            case EdgeResponse.Respawn:
            {
                break;
            }
            default:
            {
                break;
            }
        }
    }

    // Misc
    public Vector3Int Bounds { get { return guiCoordinateSystem.Bounds; } }

    [Header("Overlays")]
    [SerializeField] private float overlayThickness = 0.25f;

    // Callbacks
    public delegate void VoidDelegate_ZeroParameters();
    public delegate void VoidDelegate_IntVector2(int arg1, Vector2 arg2);
    public delegate void VoidDelegate_IntVector3(int arg1, Vector3 arg2);

    // Generation delegates
    public static event VoidDelegate_ZeroParameters Generate_MasterFunction;

    // Misc. delegates
    public static event VoidDelegate_IntVector3 WrapPoint_MasterFunction;

    // Animation components
    [SerializeField] private List<PointBehavior_Animate> animationBehaviors = new List<PointBehavior_Animate>();
    [SerializeField] private List<PointBehavior_Animate> overlayBehaviors = new List<PointBehavior_Animate>();
    [SerializeField] private List<PointBehavior_Animate> selectionBehaviors = new List<PointBehavior_Animate>();
    public void AddBehavior(PointBehavior_Animate behavior)
    {
        // TODO: Differentiate between animation, overlay, and selection
        PointBehavior_Animate animationBehavior = behavior as PointBehavior_Animate;
        PointBehavior_Animate overlayBehavior = behavior as PointBehavior_Animate;
        PointBehavior_Animate selectionBehavior = behavior as PointBehavior_Animate;
        if (animationBehavior != null)      // Animation component
        {
            animationBehaviors.Add(behavior);
        }
        else if (overlayBehavior != null)   // Overlay component
        {
            overlayBehaviors.Add(behavior);
        }
        else if (selectionBehavior != null) // Selection component
        {
            selectionBehaviors.Add(behavior);
        }
    }
    public void RemoveBehavior(PointBehavior_Animate behavior)
    {
        // TODO: Differentiate between animation, overlay, and selection
        PointBehavior_Animate animationBehavior = behavior as PointBehavior_Animate;
        PointBehavior_Animate overlayBehavior = behavior as PointBehavior_Animate;
        PointBehavior_Animate selectionBehavior = behavior as PointBehavior_Animate;
        if (animationBehavior != null)      // Animation component
        {
            animationBehaviors.Remove(behavior);
        }
        else if (overlayBehavior != null)   // Overlay component
        {
            overlayBehaviors.Remove(behavior);
        }
        else if (selectionBehavior != null) // Selection component
        {
            selectionBehaviors.Remove(behavior);
        }
    }



    private void Awake()
    {
        drawMatrix = transform.localToWorldMatrix;
        UpdateDrawParameters();
        EvaluateShapeCoordinates();
        animationBehaviors.Clear();
        foreach (Transform child in transform)
        {
            PointBehavior_Animate behavior = child.GetComponent<PointBehavior_Animate>();
            if (behavior != null)
            {
                animationBehaviors.Add(behavior);
            }
        }
        foreach (Transform child in transform)
        {
            PointBehavior_Animate behavior = child.GetComponent<PointBehavior_Animate>();
            if (behavior != null)
            {
                overlayBehaviors.Add(behavior);
            }
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        Generate_MasterFunction = Generate_Random;
        WrapPoint_MasterFunction = MovePoint_WrappedRectangular;

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
    }

    // Behaviors
    private void Update()
    {
        // Updating point behaviors
        for (int i = 0; i < positions3.Count; i++)
        {
            Vector3 movement = Vector3.zero;
            Vector3 position = positions3[i];
            foreach (PointBehavior_Animate animationBehavior in animationBehaviors)
            {
                movement += animationBehavior.UpdateBehavior(position);
            }

            WrapPoint_MasterFunction?.Invoke(i, movement);
        }

        // Updating overlay behaviors
        //  ...

        // Updating death proxies
        for (int i = wrapProxies.Count - 1; i >= 0; i--)
        {
            PointAnimProxy3 proxy = wrapProxies[i]; // TODO: Make 3D
            proxy.animationTimer += Time.deltaTime;

            if (proxy.animationTimer > proxy.animationDuration)
            {
                wrapProxies.RemoveAt(i);
            }
            else
            {
                wrapProxies[i] = proxy;
            }
        }
    }

    // Rendering
    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            // Drawing points
            foreach (Vector3 pos in positions3)
            {
                Draw.Rectangle(pos, Vector2.one * pointSize);
            }

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
            foreach (PointAnimProxy3 wrapProxy in wrapProxies)  // TODO: Make 3D
            {
                Draw.Ring(wrapProxy.position, pointSize * wrapAnimationRadiusMultiplier * wrapProxy.GetAnimationProgress());
            }
        }
    }

#region Behavior functionality
#region Generation
    public void Generate()
    {
        OnPreGeneratePoint();
        Generate_MasterFunction();
    }
    private void OnPreGeneratePoint()
    {
        positions3.Clear();
    }
    private Vector3 GenerateRandomPoint_Rectangle(Vector3 size)
    {
        return new Vector3(
            Random.Range(-size.x, size.x),
            Random.Range(-size.y, size.y),
            Random.Range(-size.z, size.z));
    }
    private Vector2 GenerateRandomPoint_Circle(float radius)
    {
        return Vector2.zero;
    }
    private Vector2 GenerateRandomPoint_Sector(float angle, float radius)
    {
        return Vector2.zero;
    }
    private void Generate_Random()
    {
        if (guiPointCount == null)
        {
            Debug.LogError("GUIIncrementSliderInput is null! Could not read point count.");
            return;
        }

        if (guiCoordinateSystem == null)
        {
            Debug.LogError("CoordinateOverlayCartesian is null! Could not read bounds.");
            return;
        }

        Vector3 bounds = (Vector3Int)guiCoordinateSystem.Bounds;
        bounds /= 2f;
        
        // TODO: Switch on bounds shape
        for (int i = 0; i < guiPointCount.SliderValue; i++)
        {
            positions3.Add(GenerateRandomPoint_Rectangle(bounds));
        }
    }
#endregion  // Generation
#region Overlay
#endregion  // Overlay
#region Select
#endregion  // Select
#region Animation
#endregion  // Animation
#endregion  // Behavior functionality

#region Internal functionality
    private void UpdateDrawParameters()
    {
        Draw.LineGeometry = lineGeometry;
        Draw.ThicknessSpace = thicknessSpace;
        Draw.Thickness = thickness;
        Draw.Matrix = drawMatrix;
    }

    private void EvaluateShapeCoordinates()
    {
        Line plusLineHorizontal = pointPrefabsParent.Find("Plus").Find("Horizontal Stroke").GetComponent<Line>();
        Line plusLineVertical = pointPrefabsParent.Find("Plus").Find("Vertical Stroke").GetComponent<Line>();
        shapeCoordinates_plus = new List<Vector2>
        {
            plusLineHorizontal.Start.normalized,
            plusLineHorizontal.End.normalized,
            plusLineVertical.Start.normalized,
            plusLineVertical.End.normalized
        };

        Line crossLineNE = pointPrefabsParent.Find("Cross").Find("NE Stroke").GetComponent<Line>();
        Line crossLineSE = pointPrefabsParent.Find("Cross").Find("SE Stroke").GetComponent<Line>();
        shapeCoordinates_cross = new List<Vector2>
        {
            crossLineNE.Start.normalized,
            crossLineNE.End.normalized,
            crossLineSE.Start.normalized,
            crossLineSE.End.normalized
        };

        shapeCoordinates_circle = 1f;

        Triangle triangle = pointPrefabsParent.Find("Triangle").GetComponent<Triangle>();
        shapeCoordinates_triangle = new List<Vector2>
        {
            triangle.A.normalized,
            triangle.B.normalized,
            triangle.C.normalized
        };

        Quad boid = pointPrefabsParent.Find("Boid").GetComponent<Quad>();
        shapeCoordinates_boid = new List<Vector2>
        {
            boid.A.normalized,
            boid.B.normalized,
            boid.C.normalized,
            boid.D.normalized
        };

        Polyline square = pointPrefabsParent.Find("Square Hollow").GetComponent<Polyline>();
        shapeCoordinates_square = new List<Vector2>
        {
            square.points[0].point.normalized,
            square.points[1].point.normalized,
            square.points[2].point.normalized,
            square.points[3].point.normalized
        };
    }
#endregion  // Internal functionality
}
