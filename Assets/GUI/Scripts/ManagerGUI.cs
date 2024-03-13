using System.Collections;
using System.Collections.Generic;
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

    [Header("Overlay")]
    [SerializeField] private GameObject componentOverlayWeb;
    [SerializeField] private GameObject componentOverlayTriangulation;
    [SerializeField] private GameObject componentOverlayConvexHull;
    [SerializeField] private GameObject componentOverlayVoronoiDiagram;
    [SerializeField] private GameObject componentOverlayDuals;
    [SerializeField] private GameObject componentOverlaySpatialPartitioning;
    [SerializeField] private GameObject componentOverlayCenterOfMass;

    [Header("Selector")]
    [SerializeField] private GameObject componentSelectorClosestPointSelector;
    [SerializeField] private GameObject componentSelectorKMeansClustering;
    [SerializeField] private GameObject componentSelectorPointSetRegistration;

    [Header("Animation")]
    [SerializeField] private GameObject componentAnimationJitter;
    [SerializeField] private GameObject componentAnimationFlocking;
    [SerializeField] private GameObject componentAnimationVectorField;
    [SerializeField] private GameObject componentAnimationWindSimulation;
    [SerializeField] private GameObject componentAnimationStrangeAttractor;
    [SerializeField] private GameObject componentAnimationLotkaVolterraEquations;
    [SerializeField] private GameObject componentAnimationSpringSystem;

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
    private Dictionary<GenerationMethod, string> GenerationMethodNames = new Dictionary<GenerationMethod, string>()
    {
        { GenerationMethod.Random, "Random" },
        //{ GenerationMethod.PoissonDisc, "Poisson Disc" },
        //{ GenerationMethod.LatticeRectangular, "Lattice (Rectangular)" },
        //{ GenerationMethod.LatticeHexagonal, "Lattice (Hexagonal)" },
        //{ GenerationMethod.DoubleSlitDistribution, "Double Slit Distribution" },
        //{ GenerationMethod.GaussianDistribution, "Gaussian Distribution" },
        //{ GenerationMethod.Import, "Import" },
    };
    private Dictionary<AnimationMethod, string> AnimationMethodNames = new Dictionary<AnimationMethod, string>()
    {
        { AnimationMethod.Jitter, "Jitter" },
        //{ AnimationMethod.Flocking, "Flocking" },
        //{ AnimationMethod.VectorField, "Vector Field" },
        //{ AnimationMethod.WindSimulation, "Wind Simulation" },
        { AnimationMethod.StrangeAttractor, "Strange Attractor" },
        //{ AnimationMethod.LotkaVolterraEquations, "Lotka-Volterra Equations" },
        //{ AnimationMethod.SpringSystem, "Spring System" },
    };
    private Dictionary<OverlayMethod, string> OverlayMethodNames = new Dictionary<OverlayMethod, string>()
    {
        { OverlayMethod.Web, "Web" },
        //{ OverlayMethod.Triangulation, "Triangulation" },
        //{ OverlayMethod.ConvexHull, "Convex Hull" },
        //{ OverlayMethod.VoronoiDiagram, "Voronoi Diagram" },
        //{ OverlayMethod.Duals, "Duals" },
        //{ OverlayMethod.SpatialPartitioning, "Spatial Partitioning" },
        { OverlayMethod.CenterOfMass, "Center of Mass" },
        //{ OverlayMethod.ClosestPointToRay, "Closest Point to Ray" },
        //{ OverlayMethod.kMeansClustering, "k-Means Clustering" },
        //{ OverlayMethod.PointSetRegistration, "Point Set Registration" },
    };



    private IEnumerator RebuildLayout_NextFrame(RectTransform rectTransform)
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        yield return null;
    }
    public void AddComponent(AnimationMethod method)
    {
        // TODO: Make more generic
        switch (method)
        {
            case AnimationMethod.Jitter:
            {
                GameObject go = Instantiate(componentAnimationJitter, componentGUIParent);
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
                GameObject go = Instantiate(componentAnimationStrangeAttractor, componentGUIParent);
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
