using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public Sprite imageCheckmark;
    public Sprite imageCollapsibleOn;
    public Sprite imageCollapsibleOff;
    public Sprite imageDropdown;
    public Sprite imageDecrement;
    public Sprite imageIncrement;

    public static ColorPalette RandomPalette(ColorPalette paletteSpriteReference)
    {
        ColorPalette palette = new ColorPalette();

        palette.colorBackgroundFill = Random.ColorHSV();
        palette.colorForegroundFill = Random.ColorHSV();
        palette.colorBackgroundPanel = Random.ColorHSV();
        palette.colorFocusFill = Random.ColorHSV();
        palette.colorFocusStroke = Random.ColorHSV();
        palette.colorFocusGlyph = Random.ColorHSV();
        palette.colorClickableNormal = Random.ColorHSV();
        palette.colorClickableHighlighted = Random.ColorHSV();
        palette.colorClickablePressed = Random.ColorHSV();
        palette.colorClickableSelected = Random.ColorHSV();
        palette.colorClickableDisabled = Random.ColorHSV();
        palette.colorFont = Random.ColorHSV();
        palette.imageCheckmark = paletteSpriteReference.imageCheckmark;
        palette.imageCollapsibleOn = paletteSpriteReference.imageCollapsibleOn;
        palette.imageCollapsibleOff = paletteSpriteReference.imageCollapsibleOff;
        palette.imageDropdown = paletteSpriteReference.imageDropdown;
        palette.imageDecrement = paletteSpriteReference.imageDecrement;
        palette.imageIncrement = paletteSpriteReference.imageIncrement;

        return palette;
    }
}
[System.Serializable]
public struct PrefabCollection_Overlay
{
    public GameObject web;
    public GameObject triangulation;
    public GameObject convexHull;
    public GameObject voronoi;
    public GameObject duals;
    public GameObject spatialPartitioning;
    public GameObject centerOfMass;
}
[System.Serializable]
public struct PrefabCollection_Selector
{
    public GameObject closestToRay;
    public GameObject kMeansClustering;
    public GameObject pointSetRegistration;
}
[System.Serializable]
public struct PrefabCollection_Animation
{
    public GameObject jitter;
    public GameObject flocking;
    public GameObject vectorField;
    public GameObject windSimulation;
    public GameObject strangeAttractor;
    public GameObject lotkaVolterra;
    public GameObject springSystem;
}
[System.Serializable]
public struct ComponentUIPair
{
    public GameObject FunctionObject;
    public GameObject UIObject;
}

public class ManagerGUI : MonoBehaviour, IColorable
{
    [Header("Color Palette")]
    [SerializeField] private Image menuBackgroundBorder;
    [SerializeField] private Image menuBackgroundFill;
    [SerializeField] private ColorPalette colorPalette;
    public ColorPalette Palette { get { return colorPalette; } }
    public ColorPalette PaletteRandom { get { return colorPalette; } }

    [Header("Component Management")]
    [SerializeField] private uint lockedComponents = 2;
    public uint LockedComponents { get { return lockedComponents; } }
    [SerializeField] private Transform componentGUIParent;
    [SerializeField] private ManagerPointSet managerPointSet;

    [Header("Components")]
    [SerializeField] private PrefabCollection_Overlay overlays;
    [SerializeField] private PrefabCollection_Selector selectors;
    [SerializeField] private PrefabCollection_Animation animations;

    [Header("Dropdowns")]
    [SerializeField] private GUIOption_DropdownGallery2 dropdownPresets;
    [SerializeField] private GUIOption_Dropdown2 dropdownBounds;
    [SerializeField] private GUIOption_Dropdown2 dropdownEdgeResponse;
    [SerializeField] private GUIOption_Dropdown2 dropdownGenerateMethod;

    // Dropdown labels
    private Dictionary<BoundsType, string> BoundsTypeNames2D = new Dictionary<BoundsType, string>()
    {
        { BoundsType.Square, "Square" },
        //{ BoundsType.Circle, "Circle" },
        //{ BoundsType.Sector, "Sector" },
    };
    private Dictionary<BoundsType, string> BoundsTypeNames3D = new Dictionary<BoundsType, string>()
    {
        { BoundsType.Cube, "Cube" },
        //{ BoundsType.Sphere, "Sphere" },
        //{ BoundsType.Cone, "Cone" },
    };
    private Dictionary<EdgeResponse, string> EdgeResponseNames = new Dictionary<EdgeResponse, string>()
    {
        { EdgeResponse.Overflow, "Overflow" },
        { EdgeResponse.Wrap, "Wrap" },
        //{ EdgeResponse.Kill, "Kill" },
        //{ EdgeResponse.Respawn, "Respawn" },
    };

    // Component lookup - initialize in OnEnable
    private Dictionary<BehaviorMethod, GUIComponent2> GUIComponents;
    private Dictionary<BehaviorMethod, PointBehavior> Behaviors;
    private Dictionary<BehaviorMethod, string> BehaviorMethodNames = new Dictionary<BehaviorMethod, string>()
    {
        // Generation
        { BehaviorMethod.Generate_Random, "Random" },
        //{ BehaviorMethod.Generate_PoissonDisc, "Poisson Disc" },
        //{ BehaviorMethod.Generate_LatticeRectangular, "Rectangular Lattice" },
        //{ BehaviorMethod.Generate_LatticeHexagonal, "Hexagonal Lattice" },
        //{ BehaviorMethod.Generate_DoubleSlitDistribution, "Double Slit Distribution" },
        //{ BehaviorMethod.Generate_GaussianDistribution, "Gaussian Distribution" },
        //{ BehaviorMethod.Generate_Import, "Import" },
        // Overlay
        //{ BehaviorMethod.Overlay_Web, "Web" },
        //{ BehaviorMethod.Overlay_Triangulation, "Triangulation" },
        //{ BehaviorMethod.Overlay_ConvexHull, "Convex Hull" },
        //{ BehaviorMethod.Overlay_Voronoi, "Voronoi Diagram" },
        //{ BehaviorMethod.Overlay_Duals, "Duals" },
        //{ BehaviorMethod.Overlay_SpatialPartitioning, "Spatial Partitioning" },
        //{ BehaviorMethod.Overlay_CenterOfMass, "Center of Mass" },
        // Selection
        //{ BehaviorMethod.Selector_ClosestToRay, "Closest to Ray" },
        //{ BehaviorMethod.Selector_kMeansClustering, "k-Means Clustering" },
        //{ BehaviorMethod.Selector_PointSetRegistration, "Point Set Registration" },
        // Animation
        { BehaviorMethod.Animate_Jitter, "Jitter" },
        //{ BehaviorMethod.Animate_Flocking, "Flocking" },
        //{ BehaviorMethod.Animate_VectorField, "Vector Field" },
        //{ BehaviorMethod.Animate_WindSimulation, "Wind Simulation" },
        { BehaviorMethod.Animate_StrangeAttractor, "Strange Attractor" },
        //{ BehaviorMethod.Animate_LotkaVolterra, "Lotka-Volterra Equations" },
        //{ BehaviorMethod.Animate_SpringSystem, "Spring System" },
    };



    private void OnEnable()
    {
        GUIComponents = new Dictionary<BehaviorMethod, GUIComponent2>()
        {
            // Overlays
            //{ BehaviorMethod.Overlay_Web, overlays.web.GetComponent<GUIComponent_OverlayWeb>() },
            //{ BehaviorMethod.Overlay_Triangulation, overlays.triangulation.GetComponent<GUIComponent_OverlayTriangulation>() },
            //{ BehaviorMethod.Overlay_ConvexHull, overlays.convexHull.GetComponent<GUIComponent_OverlayConvexHull>() },
            //{ BehaviorMethod.Overlay_VoronoiDiagram, overlays.voronoi.GetComponent<GUIComponent_OverlayVoronoiDiagram>() },
            //{ BehaviorMethod.Overlay_Duals, overlays.duals.GetComponent<GUIComponent_OverlayDuals>() },
            //{ BehaviorMethod.Overlay_SpatialPartitioning, overlays.spatialPartitioning.GetComponent<GUIComponent_OverlaySpatialPartitioning>() },
            //{ BehaviorMethod.Overlay_CenterOfMass, overlays.centerOfMass.GetComponent<GUIComponent_OverlayCenterOfMass>() },
            // Selection
            //{ BehaviorMethod.Selector_ClosestToRay, selectors.closestToRay.GetComponent<GUIComponent_SelectorClosestToRay>() },
            //{ BehaviorMethod.Selector_kMeansClustering, selectors.kMeansClustering.GetComponent<GUIComponent_SelectorKMeansClustering>() },
            //{ BehaviorMethod.Selector_PointSetRegistration, selectors.pointSetRegistration.GetComponent<GUIComponent_SelectorPointSetRegistration>() },
            // Animation
            { BehaviorMethod.Animate_Jitter, animations.jitter.GetComponent<GUIComponent_AnimateJitter2>() },
            //{ BehaviorMethod.Animate_Flocking, animations.flocking.GetComponent<GUIComponent_AnimateFlocking>() },
            //{ BehaviorMethod.Animate_VectorField, animations.vectorField.GetComponent<GUIComponent_AnimateVectorField>() },
            //{ BehaviorMethod.Animate_WindSimulation, animations.windSimulation.GetComponent<GUIComponent_AnimateWindSimulation>() },
            { BehaviorMethod.Animate_StrangeAttractor, animations.strangeAttractor.GetComponent<GUIComponent_AnimateStrangeAttractor2>() },
            //{ BehaviorMethod.Animate_LotkaVolterra, animations.lotkaVolterra.GetComponent<GUIComponent_AnimateLotkaVolterra>() },
            //{ BehaviorMethod.Animate_SpringSystem, animations.springSystem.GetComponent<GUIComponent_AnimateSpringSystem>() },
        };

        Behaviors = new Dictionary<BehaviorMethod, PointBehavior>()
        {
            // TODO: Rename to "Generate / Overlay / Selector / Animate"
            // Overlays
            //{ BehaviorMethod.Overlay_Web, overlays.web.GetComponent<PointBehavior_OverlayWeb>() },
            //{ BehaviorMethod.Overlay_Triangulation, overlays.triangulation.GetComponent<PointBehavior_OverlayTriangulation>() },
            //{ BehaviorMethod.Overlay_ConvexHull, overlays.convexHull.GetComponent<PointBehavior_OverlayConvexHull>() },
            //{ BehaviorMethod.Overlay_VoronoiDiagram, overlays.voronoi.GetComponent<PointBehavior_OverlayVoronoiDiagram>() },
            //{ BehaviorMethod.Overlay_Duals, overlays.duals.GetComponent<PointBehavior_OverlayDuals>() },
            //{ BehaviorMethod.Overlay_SpatialPartitioning, overlays.spatialPartitioning.GetComponent<PointBehavior_OverlaySpatialPartitioning>() },
            //{ BehaviorMethod.Overlay_CenterOfMass, overlays.centerOfMass.GetComponent<PointBehavior_OverlayCenterOfMass>() },
            // Selection
            //{ BehaviorMethod.Selector_ClosestToRay, selectors.closestToRay.GetComponent<PointBehavior_SelectorClosestToRay>() },
            //{ BehaviorMethod.Selector_kMeansClustering, selectors.kMeansClustering.GetComponent<PointBehavior_SelectorKMeansClustering>() },
            //{ BehaviorMethod.Selector_PointSetRegistration, selectors.pointSetRegistration.GetComponent<PointBehavior_SelectorPointSetRegistration>() },
            // Animation
            { BehaviorMethod.Animate_Jitter, animations.jitter.GetComponent<PointBehavior_AnimationJitter>() },
            //{ BehaviorMethod.Animate_Flocking, animations.flocking.GetComponent<PointBehavior_AnimationFlocking>() },
            //{ BehaviorMethod.Animate_VectorField, animations.vectorField.GetComponent<PointBehavior_AnimationVectorField>() },
            //{ BehaviorMethod.Animate_WindSimulation, animations.windSimulation.GetComponent<PointBehavior_AnimationWindSimulation>() },
            { BehaviorMethod.Animate_StrangeAttractor, animations.strangeAttractor.GetComponent<PointBehavior_AnimationStrangeAttractor>() },
            //{ BehaviorMethod.Animate_LotkaVolterra, animations.lotkaVolterra.GetComponent<PointBehavior_AnimationLotkaVolterra>() },
            //{ BehaviorMethod.Animate_SpringSystem, animations.springSystem.GetComponent<PointBehavior_AnimationSpringSystem>() },
        };
    }

    private IEnumerator RebuildLayout_NextFrame(RectTransform rectTransform)
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        yield return null;
    }
    public void AddComponent(BehaviorMethod method)
    {
        // TODO: Make more generic
        // TODO: Make dictionary of matching GUI and Behavior types

        // CONTINUE: connect prefabs into here somehow
        //GameObject go = Instantiate(GUIComponents[method])


        switch (method)
        {
            case AnimationMethod.Jitter:
            {
                GameObject go = Instantiate(animations.jitter, componentGUIParent);
                go.transform.SetSiblingIndex(go.transform.GetSiblingIndex() - 1);
                GUIComponent_AnimateJitter2 compGUI = go.GetComponent<GUIComponent_AnimateJitter2>();
                compGUI.Manager_GUI = this;
                PointBehavior_AnimationJitter compFunction = go.GetComponent<PointBehavior_AnimationJitter>();
                compFunction.ManagerPointSet = managerPointSet;
                managerPointSet.AddBehavior(compFunction);
                // TODO: Improve safety on these
                // Rebuilds Content layout
                StartCoroutine(RebuildLayout_NextFrame(componentGUIParent.GetComponent<RectTransform>()));
                // Rebuilds Identifier buttons layout
                StartCoroutine(RebuildLayout_NextFrame(go.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>()));
                // Rebuilds Organization buttons layout
                StartCoroutine(RebuildLayout_NextFrame(go.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>()));

                break;
            }
            case AnimationMethod.StrangeAttractor:
            {
                GameObject go = Instantiate(animations.strangeAttractor, componentGUIParent);
                go.transform.SetSiblingIndex(go.transform.GetSiblingIndex() - 1);
                GUIComponent_AnimateStrangeAttractor2 compGUI = go.GetComponent<GUIComponent_AnimateStrangeAttractor2>();
                compGUI.Manager_GUI = this;
                PointBehavior_AnimationStrangeAttractor compFunction = go.GetComponent<PointBehavior_AnimationStrangeAttractor>();
                compFunction.ManagerPointSet = managerPointSet;
                managerPointSet.AddBehavior(compFunction);
                // TODO: Improve safety on these
                // Rebuilds Content layout
                StartCoroutine(RebuildLayout_NextFrame(componentGUIParent.GetComponent<RectTransform>()));
                // Rebuilds Identifier buttons layout
                StartCoroutine(RebuildLayout_NextFrame(go.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>()));
                // Rebuilds Organization buttons layout
                StartCoroutine(RebuildLayout_NextFrame(go.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>()));

                break;
            }
        }
    }
    public void RemoveComponent(GameObject component)
    {
        // TODO: Make generic
        managerPointSet.RemoveBehavior(component.GetComponent<PointBehavior_Animate>());
        Destroy(component);
        // ...
        StartCoroutine(RebuildLayout_NextFrame(componentGUIParent.GetComponent<RectTransform>()));
    }

    public void ApplyColorPalette(ColorPalette colorPalette)
    {
        IColorable.ApplyColorPalette_Image(menuBackgroundBorder, colorPalette.colorBackgroundPanel);
        IColorable.ApplyColorPalette_Image(menuBackgroundFill, colorPalette.colorBackgroundFill);
        // TODO: Scrollbar
        foreach (Transform component in componentGUIParent)
        {
            component.GetComponent<IColorable>().ApplyColorPalette(colorPalette);
        }
    }

    public void ApplyDropdownItems()
    {
        // TODO: Remove or relocate?
        // TODO: Swap between 2D and 3D
        dropdownBounds.OverwriteDropdownEntries<BoundsType>(BoundsTypeNames2D);
        dropdownEdgeResponse.OverwriteDropdownEntries<EdgeResponse>(EdgeResponseNames);
        dropdownGenerateMethod.OverwriteDropdownEntries<GenerationMethod>(GenerationMethodNames);
    }
}
