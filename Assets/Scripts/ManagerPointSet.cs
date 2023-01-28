using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ManagerPointSet : MonoBehaviour
{
    [SerializeField] private List<Transform> points = new List<Transform>();

    [Header("Spawning")]
    [SerializeField] private GameObject pointStatic;
    [SerializeField] private GameObject pointMoving;
    [SerializeField] private GameObject pointSimulated;
    [SerializeField] private Transform pointsParent;
    [SerializeField, Range(0f, 2f)] private float pointMeshScale = 1f;
    [SerializeField, Range(0, 1000000)] private int density = 256;
    public int Density { get { return density; } set { density = value; } }
    private bool pendingDelete = false;
    [SerializeField] private Vector3 spawnPointSafeZone = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 spawnPointBoundingBoxOctant = new Vector3(10f, 10f, 10f);

    // Settings
    [Header("Jitter")]
    [SerializeField] private bool animateJitter = false;
    public bool AnimateJitter
    {
        get { return animateJitter; }
        set
        {
            animateJitter = value;
        }
    }
    [SerializeField, Range(0f, 10f)] private float animateJitterSpeedMax = 1f;
    [SerializeField, Range(0f, 1f)] private float animateJitterSmoothing = 0f;

    //[Header("Life and Death")]
    //[Header("Spring Simulation")]
    //[Header("Flocking")]
    //[Header("Vector Field")]
    //[Header("Wind Simulation")]
    //[Header("Strange Attractor")]
    //[Header("Lotka-Volterra Equations")]

    [SerializeField] private bool is2D = true;
    public bool Is2D
    {
        get { return is2D; }
        set
        {
            if (is2D == value)
                return;

            is2D = value;
            ChangeDimension(is2D);
        }
    }

    // Callbacks
    public delegate void GenerateFunction();
    public delegate void OverlayFunction();
    public delegate void AnimateFunction();
    public delegate void SelectFunction();

    // Generation delegates
    public static event GenerateFunction Generate_MasterFunction;

    // Overlay delegates
    public static event OverlayFunction Overlay_MasterFunction;

    // Animation delegates
    public static event AnimateFunction Animate_MasterFunction;
    public static event AnimateFunction Animate_Jitter;

    // Selection delegates
    public static event SelectFunction Select_MasterFunction;



    private void Awake()
    {
        if (pointsParent == null)
            pointsParent = GameObject.Find("Points Parent").transform;

        if (pointStatic == null ||
            pointMoving == null ||
            pointSimulated == null ||
            pointsParent == null)
        {
            gameObject.SetActive(false);
        }

        points.Clear();

        Camera cam = Camera.main;
        if (cam.orthographic)
        {
            spawnPointBoundingBoxOctant = new Vector3(
                (cam.orthographicSize - spawnPointSafeZone.x),
                (cam.orthographicSize - spawnPointSafeZone.y),
                (cam.orthographicSize - spawnPointSafeZone.z));
        }

        Generate_MasterFunction = GeneratePoints_RandomDisordered;
    }

    private void FixedUpdate()
    {
        // If animating or otherwise moving, also re-run all related overlay functions, and update their corresponding visualizations

        if (Animate_MasterFunction != null && !pendingDelete)
        {
            Animate_MasterFunction();
        }
    }

    private void ChangeDimension(bool is2D)
    {
        if (is2D)
        {
            Animate_Jitter = Animate_Jitter_2D;
        }
        else
        {
            Animate_Jitter = Animate_Jitter_3D;
        }
    }

    private void OnEnable()
    {
        // Subscribe all relevant delegates
    }

    private void OnDisable()
    {
        // Ubsubscribe all relevant delegates
    }

    public void Animate_Jitter_2D()
    {
        foreach (Transform point in points)
        {
            Vector3 randomVelocity = new Vector3(
                UnityEngine.Random.Range(-1.0f, 1.0f),
                0f,
                UnityEngine.Random.Range(-1.0f, 1.0f)) * animateJitterSpeedMax * Time.fixedDeltaTime;
            point.position = Vector3.Lerp(point.position, point.position + randomVelocity, 1f - animateJitterSmoothing);
        }
    }

    public void Animate_Jitter_3D()
    {
        foreach (Transform point in points)
        {
            Vector3 randomVelocity = new Vector3(
                UnityEngine.Random.Range(-1.0f, 1.0f),
                UnityEngine.Random.Range(-1.0f, 1.0f),
                UnityEngine.Random.Range(-1.0f, 1.0f)) * animateJitterSpeedMax * Time.fixedDeltaTime;
            point.position = Vector3.Lerp(point.position, point.position + randomVelocity, 1f - animateJitterSmoothing);
        }
    }

    public IEnumerator GeneratePoints()
    {
        yield return StartCoroutine(DeletePoints());
        if (Generate_MasterFunction != null)
            Generate_MasterFunction();
    }

    public IEnumerator DeletePoints()
    {
        pendingDelete = true;

        // Ensuring no functions are trying to access soon-to-be-deleted game objects
        // This assumes everything directly referencing them are in FixedUpdate()
        yield return new WaitForFixedUpdate();

        foreach (Transform child in pointsParent)
        {
            Destroy(child.gameObject);
        }
        points.Clear();

        pendingDelete = false;
    }

    public Vector3 GenerateRandomVector3_BoundingBox(Vector3 boundingBoxOctant, Func<Vector3, Vector3> biasFunction = null)
    {
        float randomX = UnityEngine.Random.Range(-boundingBoxOctant.x, boundingBoxOctant.x);
        float randomY = UnityEngine.Random.Range(-boundingBoxOctant.y, boundingBoxOctant.y);
        float randomZ = UnityEngine.Random.Range(-boundingBoxOctant.z, boundingBoxOctant.z);
        Vector3 randomVector = new Vector3(randomX, randomY, randomZ);

        if (biasFunction != null)
        {
            randomVector = biasFunction(randomVector);
        }

        return randomVector;
    }

    public Vector3 SquareResult(Vector3 input)
    {
        // TODO: Move into MathUtils

        float x = input.x;
        float y = input.y;
        float z = input.z;

        int signX = x < 0 ? -1 : 1;
        int signY = y < 0 ? -1 : 1;
        int signZ = z < 0 ? -1 : 1;

        float magnitudeMax = spawnPointBoundingBoxOctant.magnitude;
        x = MathUtils.Remap(0f, magnitudeMax, 0f, 1f, x);
        y = MathUtils.Remap(0f, magnitudeMax, 0f, 1f, y);
        z = MathUtils.Remap(0f, magnitudeMax, 0f, 1f, z);

        x *= x * signX;
        y *= y * signY;
        z *= z * signZ;

        x = MathUtils.Remap(0f, 1f, 0f, magnitudeMax, x);
        y = MathUtils.Remap(0f, 1f, 0f, magnitudeMax, y);
        z = MathUtils.Remap(0f, 1f, 0f, magnitudeMax, z);

        return new Vector3(x, y, z);
    }

    private void GeneratePoint(GameObject prefab, Vector3 position, Vector3 forwardOrientation = new Vector3(), float scale = 1f)
    {
        if (forwardOrientation == Vector3.zero) forwardOrientation = Vector3.forward;
        GameObject go = Instantiate(prefab, position, Quaternion.LookRotation(forwardOrientation), pointsParent);
        go.transform.localScale = new Vector3(scale, scale, scale);
        points.Add(go.transform);
    }

    public void GeneratePoints_RandomDisordered()
    {
        for (int i = 0; i < Density; i++)
        {
            Vector3 point = GenerateRandomVector3_BoundingBox(spawnPointBoundingBoxOctant, SquareResult);
            // Optionally pass through a bias curve

            GeneratePoint(pointStatic, point, Vector3.forward, pointMeshScale);
        }
    }

    public void GeneratePoints_RandomBlueNoise(uint Density, float DiskRadius, uint GenerationIterationLimit, uint TerminationLimit)
    {
        // TODO: Cache parameters instead in order to conform to delegate signatures

        for (int i = 0, termination = 0; i < Density; i++)
        {
            //float
        }
    }
}
