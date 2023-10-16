using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ColorPalette
{
    public Color colorBackgroundFill;
    public Color colorForegroundFill;
    public Color colorBackgroundPanel;
    public Color colorFocusFill;
    public Color colorFocusStroke;
    public Color colorFocusGlyph;
    public Color colorClickableNormal;
    public Color colorClickableHighlighted;
    public Color colorClickablePressed;
    public Color colorClickableSelected;
    public Color colorClickableDisabled;
    public Color colorFont;
}
[System.Serializable]
public struct ComponentUIPair
{
    public GameObject FunctionObject;
    public GameObject UIObject;
}

public class GUIManager : MonoBehaviour
{
    [Header("Color Palette")]
    [SerializeField] private Image menuBorderColorRef;
    [SerializeField] private Image menuBackgroundColorRef;
    [SerializeField] private List<GUIComponent> componentsColorRef;
    [SerializeField] private ColorPalette colorPalette;

    [Header("Component Management")]
    [SerializeField] private uint lockedComponents = 2;
    public uint LockedComponents { get { return lockedComponents; } }
    [SerializeField] private Transform componentGUIParent;
    [SerializeField] private Transform componentFunctionParent;
    private List<ComponentUIPair> components;
    [SerializeField] TMP_Dropdown addComponentDropdown;
    [SerializeField] private GameObject emptyPrefab;
    // Overlay
    [SerializeField] private GameObject componentOverlayWeb;
    [SerializeField] private GameObject componentOverlayTriangulation;
    [SerializeField] private GameObject componentOverlayConvexHull;
    [SerializeField] private GameObject componentOverlayVoronoiDiagram;
    [SerializeField] private GameObject componentOverlayDuals;
    [SerializeField] private GameObject componentOverlaySpatialPartitioning;
    [SerializeField] private GameObject componentOverlayCenterOfMass;
    // Selector
    [SerializeField] private GameObject componentSelectorClosestPointSelector;
    [SerializeField] private GameObject componentSelectorKMeansClustering;
    [SerializeField] private GameObject componentSelectorPointSetRegistration;
    // Animation
    [SerializeField] private GameObject componentAnimationJitter;
    [SerializeField] private GameObject componentAnimationFlocking;
    [SerializeField] private GameObject componentAnimationVectorField;
    [SerializeField] private GameObject componentAnimationWindSimulation;
    [SerializeField] private GameObject componentAnimationStrangeAttractor;
    [SerializeField] private GameObject componentAnimationLotkaVolterraEquations;
    [SerializeField] private GameObject componentAnimationSpringSystem;

    [Header("Dropdown Labels")]
    [SerializeField]
    private Dictionary<BoundsType, string> BoundsTypeNames2D = new Dictionary<BoundsType, string>()
    {
        { BoundsType.Square, "Square" },
        { BoundsType.Circle, "Circle" },
        { BoundsType.Sector, "Sector" },
    };
    [SerializeField]
    private Dictionary<BoundsType, string> BoundsTypeNames3D = new Dictionary<BoundsType, string>()
    {
        { BoundsType.Cube, "Cube" },
        { BoundsType.Sphere, "Sphere" },
        { BoundsType.Cone, "Cone" },
    };
    [SerializeField]
    private Dictionary<EdgeResponse, string> EdgeResponseNames = new Dictionary<EdgeResponse, string>()
    {
        { EdgeResponse.Overflow, "Overflow" },
        { EdgeResponse.Wrap, "Wrap" },
        { EdgeResponse.Kill, "Kill" },
        { EdgeResponse.Respawn, "Respawn" },
    };
    [SerializeField]
    private Dictionary<GenerationMethod, string> GenerationMethodNames = new Dictionary<GenerationMethod, string>()
    {
        { GenerationMethod.Random, "Random" },
        { GenerationMethod.PoissonDisc, "Poisson Disc" },
        { GenerationMethod.LatticeRectangular, "Lattice (Rectangular)" },
        { GenerationMethod.LatticeHexagonal, "Lattice (Hexagonal)" },
        { GenerationMethod.DoubleSlitDistribution, "Double Slit Distribution" },
        { GenerationMethod.GaussianDistribution, "Gaussian Distribution" },
        { GenerationMethod.Import, "Import" },
    };
    [SerializeField]
    private Dictionary<OverlayMethod, string> OverlayMethodNames = new Dictionary<OverlayMethod, string>()
    {
        { OverlayMethod.Web, "Web" },
        { OverlayMethod.Triangulation, "Triangulation" },
        { OverlayMethod.ConvexHull, "Convex Hull" },
        { OverlayMethod.VoronoiDiagram, "Voronoi Diagram" },
        { OverlayMethod.Duals, "Duals" },
        { OverlayMethod.SpatialPartitioning, "Spatial Partitioning" },
        { OverlayMethod.CenterOfMass, "Center of Mass" },
    };
    [SerializeField]
    private Dictionary<SelectionMethod, string> SelectionMethodNames = new Dictionary<SelectionMethod, string>()
    {
        { SelectionMethod.ClosestPointToRay, "Closest Point to Ray" },
        { SelectionMethod.kMeansClustering, "k-Means Clustering" },
        { SelectionMethod.PointSetRegistration, "Point Set Registration" },
    };
    [SerializeField]
    private Dictionary<AnimationMethod, string> AnimationMethodNames = new Dictionary<AnimationMethod, string>()
    {
        { AnimationMethod.Jitter, "Jitter" },
        { AnimationMethod.Flocking, "Flocking" },
        { AnimationMethod.VectorField, "Vector Field" },
        { AnimationMethod.WindSimulation, "Wind Simulation" },
        { AnimationMethod.StrangeAttractor, "Strange Attractor" },
        { AnimationMethod.LotkaVolterraEquations, "Lotka-Volterra Equations" },
        { AnimationMethod.SpringSystem, "Spring System" },
    };



    public void AddComponent(int index)
    {
        int firstOverlayIndex = 0;
        int firstSelectionIndex = firstOverlayIndex + OverlayMethodNames.Count;
        int firstAnimationIndex = firstSelectionIndex + SelectionMethodNames.Count;
        int lastAnimationIndex = firstAnimationIndex + AnimationMethodNames.Count - 1;

        if (index < 0 || index > lastAnimationIndex)
        {
            Debug.LogWarning("Failed to add component - index out of dropdown range.");
            return;
        }

        // Overlays
        if (index < firstSelectionIndex)
        {
            AddComponent((OverlayMethod)index);
        }
        // Selections
        else if (index < firstAnimationIndex)
        {
            AddComponent((SelectionMethod)(index - OverlayMethodNames.Count));
        }
        // Animations
        else if (index <= lastAnimationIndex)
        {
            AddComponent((AnimationMethod)(index - OverlayMethodNames.Count - SelectionMethodNames.Count));
        }

        addComponentDropdown.SetValueWithoutNotify(-1);

        // TODO: Disable edge reorganizational buttons (can't move up if first component after Settings and Generate, can't move down if last element)
    }
    private void AddComponent(OverlayMethod method)
    {
        switch (method)
        {
            case OverlayMethod.Web:
            {
                GameObject go = Instantiate(emptyPrefab, componentFunctionParent);
                PointBehavior_OverlayWeb comp = go.AddComponent<PointBehavior_OverlayWeb>();
                AddGUIComponent(componentOverlayWeb, comp);
                componentFunctionParent.GetComponent<ManagerPointSet>().AddBehavior(comp);
                break;
            }
        }

        // TODO: Set button disability in GUI (also on remove)
    }
    private void AddComponent(SelectionMethod method)
    {

    }
    private void AddComponent(AnimationMethod method)
    {
        switch (method)
        {
            case AnimationMethod.Jitter:
            {
                GameObject go = Instantiate(emptyPrefab, componentFunctionParent);
                go.name = "Animation - Jitter";
                PointBehavior_AnimationJitter comp = go.AddComponent<PointBehavior_AnimationJitter>();
                AddGUIComponent(componentAnimationJitter, comp);
                componentFunctionParent.GetComponent<ManagerPointSet>().AddBehavior(comp);
                break;
            }
            case AnimationMethod.StrangeAttractor:
            {
                GameObject go = Instantiate(emptyPrefab, componentFunctionParent);
                go.name = "Animation - Strange Attractor";
                PointBehavior_AnimationStrangeAttractor comp = go.AddComponent<PointBehavior_AnimationStrangeAttractor>();
                AddGUIComponent(componentAnimationStrangeAttractor, comp);
                componentFunctionParent.GetComponent<ManagerPointSet>().AddBehavior(comp);
                break;
            }
        }
    }
    private void AddGUIComponent(GameObject prefab, PointBehavior_Animate behaviorReference)
    {
        GameObject go = Instantiate(prefab, componentGUIParent);
        go.GetComponent<GUIComponent>().Behavior = behaviorReference;
        // Setting as second-to-last sibling (with the "Add Component" button being the last)
        go.transform.SetSiblingIndex(componentGUIParent.childCount - 2);
        // TODO: Add some reference from the GUI back to the behavior object
    }

    public void ApplyColorPalette(ColorPalette palette)
    {
        menuBorderColorRef.color = palette.colorForegroundFill;
        EditorUtility.SetDirty(menuBorderColorRef);
        menuBackgroundColorRef.color = palette.colorBackgroundPanel;
        EditorUtility.SetDirty(menuBackgroundColorRef);
        foreach (GUIComponent component in componentsColorRef)
        {
            component.ApplyColorPalette(palette);
        }
    }
    public void ApplyColorPalette()
    {
        ApplyColorPalette(colorPalette);
    }
    public void ApplyRandomColorPalette()
    {
        ColorPalette palette = new ColorPalette();
        palette.colorBackgroundFill = Random.ColorHSV();
        palette.colorForegroundFill = Random.ColorHSV();
        palette.colorFocusFill = Random.ColorHSV();
        palette.colorFocusStroke = Random.ColorHSV();
        palette.colorFocusGlyph = Random.ColorHSV();
        palette.colorClickableNormal = Random.ColorHSV();
        palette.colorClickableHighlighted = Random.ColorHSV();
        palette.colorClickablePressed = Random.ColorHSV();
        palette.colorClickableSelected = Random.ColorHSV();
        palette.colorClickableDisabled = Random.ColorHSV();
        palette.colorFont = Random.ColorHSV();
        ApplyColorPalette(palette);
    }
}
