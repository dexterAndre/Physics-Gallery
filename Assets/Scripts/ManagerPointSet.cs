using Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.UI;

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
[System.Serializable]
public struct PointAnimProxy2
{
    public PointAnimProxy2(Vector2 inPosition = default(Vector2), float inAnimationDuration = 1f)
    {
        position = inPosition;
        animationDuration = inAnimationDuration;
        animationTimer = 0f;
    }

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

    public Vector2 position;
    public float animationDuration;
    public float animationTimer;
}



public class ManagerPointSet : ImmediateModeShapeDrawer
{
    [SerializeField] private List<Vector2> positions2 = new List<Vector2>();
    public void SetPoint(int index, Vector2 point)
    {
        positions2[index] = point;
    }
    public void MovePoint(int index, Vector2 displacement)
    {
        Vector2 newPos = positions2[index] + displacement * Time.deltaTime;
        positions2[index] = newPos;
    }
    public void MovePoint_WrappedRectangular(int index, Vector2 displacement)
    {
        Vector2 newPos = positions2[index] + displacement * Time.deltaTime;
        Vector2 wrapAnimationPosition = newPos;

        // Wrapping
        Vector2 boundsHalf = guiCoordinateSystem.BoundsHalf;
        Vector2 interval = (Vector3)guiCoordinateSystem.Bounds;
        bool shouldWrap = false;
        if (newPos.x < -boundsHalf.x)
        {
            newPos.x += interval.x;
            shouldWrap = true;
            wrapAnimationPosition = MathUtils.ClosestPointToLine(wrapAnimationPosition, guiCoordinateSystem.GridCorners[2].point, Vector2.up);
        }
        else if (newPos.x > boundsHalf.x)
        {
            newPos.x -= interval.x;
            shouldWrap = true;
            wrapAnimationPosition = MathUtils.ClosestPointToLine(wrapAnimationPosition, guiCoordinateSystem.GridCorners[3].point, Vector2.up);
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

        if (shouldWrap)
        {
            wrapProxies.Add(new PointAnimProxy2(wrapAnimationPosition, wrapAnimationDuration));
        }

        positions2[index] = newPos;
    }
    public int PointCount()
    {
        return positions2.Count;
    }
    [SerializeField] private List<PosRot2> posrots2 = new List<PosRot2>();
    [SerializeField] private List<Vector3> positions3 = new List<Vector3>();
    [SerializeField] private List<PosRot3> posrots3 = new List<PosRot3>();
    [SerializeField] private List<PointAnimProxy2> wrapProxies = new List<PointAnimProxy2>();

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

    [Header("Overlays")]
    [SerializeField] private float overlayThickness = 0.25f;

    // Callbacks
    public delegate void VoidDelegate_ZeroParameters();
    public delegate void VoidDelegate_IntVector2(int arg1, Vector2 arg2);

    // Generation delegates
    public static event VoidDelegate_ZeroParameters Generate_MasterFunction;

    // Misc. delegates
    public static event VoidDelegate_IntVector2 WrapPoint_MasterFunction;

    // Animation components
    [SerializeField] private List<PointBehaviorAnimation2> animationBehaviors = new List<PointBehaviorAnimation2>();

    // Overlay components
    [SerializeField] private List<PointBehaviorOverlay2> overlayBehaviors = new List<PointBehaviorOverlay2>();



    private void Awake()
    {
        drawMatrix = transform.localToWorldMatrix;
        UpdateDrawParameters();
        EvaluateShapeCoordinates();
        animationBehaviors.Clear();
        foreach (Transform child in transform)
        {
            PointBehaviorAnimation2 behavior = child.GetComponent<PointBehaviorAnimation2>();
            if (behavior != null)
            {
                animationBehaviors.Add(behavior);
            }
        }
        foreach (Transform child in transform)
        {
            PointBehaviorOverlay2 behavior = child.GetComponent<PointBehaviorOverlay2>();
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
        for (int i = 0; i < positions2.Count; i++)
        {
            Vector2 movement = Vector2.zero;
            foreach (PointBehaviorAnimation2 animationBehavior in animationBehaviors)
            {
                movement += animationBehavior.UpdateBehavior();
            }

            WrapPoint_MasterFunction?.Invoke(i, movement);
        }

        // Updating overlay behaviors
        //  ...

        // Updating death proxies
        for (int i = wrapProxies.Count - 1; i >= 0; i--)
        {
            PointAnimProxy2 proxy = wrapProxies[i];
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
            foreach (Vector2 pos in positions2)
            {
                Draw.Rectangle(pos, Vector2.one * pointSize);
            }

            // Drawing overlays
            //      n^2 web
            Draw.Thickness = overlayThickness;
            for (int i = 0; i < positions2.Count - 1; i++)
            {
                for (int j = i; j < positions2.Count; j++)
                {
                    Draw.Line(positions2[i], positions2[j]);
                }
            }

            // Drawing wrap event indicator animations
            Draw.Thickness = wrapAnimationThickness;
            foreach (PointAnimProxy2 wrapProxy in wrapProxies)
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
        positions2.Clear();
    }
    private Vector2 GenerateRandomPoint_Rectangle(Vector2 size)
    {
        return new Vector2(
            Random.Range(-size.x, size.x),
            Random.Range(-size.y, size.y));
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

        Vector2 bounds = (Vector2Int)guiCoordinateSystem.Bounds;
        bounds /= 2f;
        
        // TODO: Switch on bounds shape
        for (int i = 0; i < guiPointCount.SliderValue; i++)
        {
            positions2.Add(GenerateRandomPoint_Rectangle(bounds));
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
