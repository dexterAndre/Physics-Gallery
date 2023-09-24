using Shapes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Settings")]
    [SerializeField] private int subdivisions = 2;
    private int Subdivisions
    {
        get { return subdivisions; }
        set
        {
            subdivisions = value;
            RecreateTicks();
        }
    }
    [SerializeField, Tooltip("Min and Max length for ticks.")]
    Vector2 tickLength = new Vector2(0.05f, 0.05f);
    private Vector2 TickLength
    {
        get { return tickLength; }
        set
        {
            tickLength = value;
            //RecalculateTicks();
        }
    }
    [SerializeField, Tooltip("Does tickLength represent percentage of bounds?")]
    private bool relativeLength = false;

    [Header("References")]
    [SerializeField] private List<Transform> tickParents = new List<Transform>();
    [SerializeField] private VisualizerCameraController cameraController;
    [SerializeField, Tooltip("Boundary visualizer. Also used to reference graphics settings from, for example in subobjects like ticks or points of interest.")]
    private Polyline gridCorners;
    [SerializeField] private GameObject lineObject;



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

        if (tickParents.Count <= 0)
        {
            for (int i = 0; i < 4; i++)
            {
                tickParents.Add(transform.GetChild(0).GetChild(i).transform);
            }
        }

        SetGridCorners();
        StartRecreatingTicks();
    }

    private void SetGridCorners()
    {
        if (gridCorners == null || cameraController == null)
        {
            Debug.LogError("Error: Could not find Polyline on current GameObject or VisualizerCameraController on parent GameObject. Disabling this CoordinateOverlayCartesian component.");
        }

        gridCorners.points.Clear();
        Vector3 refVec = cameraController.BoundarySize;
        gridCorners.AddPoint(new Vector3(refVec.x, refVec.y, 0f));
        gridCorners.AddPoint(new Vector3(-refVec.x, refVec.y, 0f));
        gridCorners.AddPoint(new Vector3(-refVec.x, -refVec.y, 0f));
        gridCorners.AddPoint(new Vector3(refVec.x, -refVec.y, 0f));
    }

    public void StartRecreatingTicks()
    {
        StartCoroutine(RecreateTicks());
    }

    private IEnumerator RecreateTicks()
    {
        // Clears previous ticks
        tickParents.Clear();
        DestroyAllChildren();
        yield return new WaitForEndOfFrame();

        // Creates new tick gameObjects
        int tickCount = TicksPerSubdivision(subdivisions);

        Transform eastParent = transform.GetChild(0).Find("East");
        Transform northParent = transform.GetChild(0).Find("North");
        Transform westParent = transform.GetChild(0).Find("West");
        Transform southParent = transform.GetChild(0).Find("South");
        CreateLineChildren(Direction.East, tickCount, eastParent);
        CreateLineChildren(Direction.North, tickCount, northParent);
        CreateLineChildren(Direction.West, tickCount, westParent);
        CreateLineChildren(Direction.South, tickCount, southParent);
    }

    private GameObject CreateLineChild(Transform parent, Vector3 position = default, Quaternion rotation = default)
    {
        GameObject go = Instantiate(lineObject, position, rotation, parent);
        go.transform.parent = parent;
        return go;
    }

    private void CreateLineChildren(Direction side, int tickCount, Transform tickParent)
    {
        for (int i = 0; i < tickCount; i++)
        {
            float linearProgress = (float)i / (tickCount - 1);
            GameObject child = CreateLineChild(tickParent);
            child.transform.SetParent(tickParent);  // TODO: Does this work?
            Vector3Pair cornerPoints = SelectGridCorners(side);
            Vector3 position = cornerPoints.A + linearProgress * (cornerPoints.B - cornerPoints.A);
            float magnitude = CalculateTickLength(tickLength.x, tickLength.y, i, subdivisions);
            if (relativeLength)
            {
                bool isLateral = side == Direction.East || side == Direction.West;
                magnitude = magnitude / (isLateral ? cameraController.BoundarySize.x : cameraController.BoundarySize.y);
            }
            Vector3 direction = SelectDirectionVector(side);
            Vector3Pair linePoints = new Vector3Pair(position, direction, magnitude);
            Line line = child.GetComponent<Line>();
            line.Start = linePoints.A;
            line.End = linePoints.B;
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

    private float CalculateTickLength(float minLength, float maxLength, int tickIndex, int subdivision)
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
        int subdivisionTester = subdivision;
        while (subdivisionTester >= 0)
        {
            // Checking what level of subdivision current tick lies within
            int currentTest = CalculateTickCount(subdivisionTester);
            if (tickIndex % currentTest == 0)
            {
                subdivisionDepthFraction = (float)currentTest / CalculateTickCount(subdivision);
                break;
            }
            subdivisionTester--;
        }

        return minLength + subdivisionDepthFraction * (maxLength - minLength);
    }

    private int CalculateTickCount(int subdivision)
    {
        return Mathf.RoundToInt(Mathf.Pow(2, subdivision));
    }

    private int TicksPerSubdivision(int subdivision)
    {
        return Mathf.RoundToInt(Mathf.Pow(2, subdivision)) + 1;
    }

    private void DestroyAllChildren()
    {
        Transform tickGrandparent = transform.GetChild(0);  // Child under this behavior
        for (int tickParentIndex = 0; tickParentIndex < tickGrandparent.childCount; tickParentIndex++)
        {
            // Iterating backwards and destroying all children
            // Note: Must wait until end of frame for these changes to take effect!
            for (int tickChildIndex = tickGrandparent.GetChild(tickParentIndex).childCount - 1; tickChildIndex >= 0; tickChildIndex--)
            {
                Destroy(tickGrandparent.GetChild(tickChildIndex).gameObject);
            }
        }
    }
}
