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

public class Manager_GUI : MonoBehaviour, IColorable
{
    [Header("Color Palette")]
    [SerializeField] private Image menuBackgroundBorder;
    [SerializeField] private Image menuBackgroundFill;
    [SerializeField] private ColorPalette colorPalette;
    public ColorPalette Palette { get { return colorPalette; } }
    public ColorPalette PaletteRandom { get { return colorPalette; } }

    [Header("Component Management")]
    [SerializeField] private GUIComponent_Settings componentSettings;
    public GUIComponent_Settings ComponentSettings { get { return componentSettings; } }
    [SerializeField] private GUIComponent_Generate componentGenerate;
    [SerializeField] private Transform componentParentAnimation;
    [SerializeField] private Transform componentParentOverlay;
    [SerializeField] private Transform componentParentSelector;
    [SerializeField] private GUIComponent_AddComponent componentAddComponent;
    [SerializeField] private Manager_PointSet managerPointSet;

    [Header("Components")]
    [SerializeField] private PrefabCollection_Animation animations;
    [SerializeField] private PrefabCollection_Overlay overlays;
    [SerializeField] private PrefabCollection_Selector selectors;

    [Header("Dropdowns")]
    //[SerializeField] private GUIOption_DropdownGallery dropdownPresets;
    [SerializeField] private GUIOption_Dropdown dropdownBounds;
    [SerializeField] private GUIOption_Dropdown dropdownEdgeResponse;
    [SerializeField] private GUIOption_Dropdown dropdownGenerateMethod;

    // Dropdown labels


    private Dictionary<BehaviorMethod, string> nameList_Components = new Dictionary<BehaviorMethod, string>()
    {
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
    public Dictionary<BehaviorMethod, string> NameList_Components { get { return nameList_Components; } }

    // Component lookup - initialize in OnEnable
    private Dictionary<BehaviorMethod, GameObject> ComponentsPrefab;
    private Dictionary<BehaviorMethod, GUIComponent> ComponentsGUI;
    private Dictionary<BehaviorMethod, PointBehavior> ComponentsBehavior;



    private void OnEnable()
    {
        ComponentsPrefab = new Dictionary<BehaviorMethod, GameObject>()
        {
            // Overlays
            //{ BehaviorMethod.Overlay_Web, overlays.web },
            //{ BehaviorMethod.Overlay_Triangulation, overlays.triangulation },
            //{ BehaviorMethod.Overlay_ConvexHull, overlays.convexHull },
            //{ BehaviorMethod.Overlay_VoronoiDiagram, overlays.voronoi },
            //{ BehaviorMethod.Overlay_Duals, overlays.duals },
            //{ BehaviorMethod.Overlay_SpatialPartitioning, overlays.spatialPartitioning },
            //{ BehaviorMethod.Overlay_CenterOfMass, overlays.centerOfMass },
            // Selection
            //{ BehaviorMethod.Selector_ClosestToRay, selectors.closestToRay },
            //{ BehaviorMethod.Selector_kMeansClustering, selectors.kMeansClustering },
            //{ BehaviorMethod.Selector_PointSetRegistration, selectors.pointSetRegistration },
            // Animation
            { BehaviorMethod.Animate_Jitter, animations.jitter },
            //{ BehaviorMethod.Animate_Flocking, animations.flocking },
            //{ BehaviorMethod.Animate_VectorField, animations.vectorField },
            //{ BehaviorMethod.Animate_WindSimulation, animations.windSimulation },
            { BehaviorMethod.Animate_StrangeAttractor, animations.strangeAttractor },
            //{ BehaviorMethod.Animate_LotkaVolterra, animations.lotkaVolterra },
            //{ BehaviorMethod.Animate_SpringSystem, animations.springSystem },
        };

        ComponentsGUI = new Dictionary<BehaviorMethod, GUIComponent>()
        {
            // Animation
            { BehaviorMethod.Animate_Jitter, animations.jitter.GetComponent<GUIComponent_AnimateJitter>() },
            //{ BehaviorMethod.Animate_Flocking, animations.flocking.GetComponent<GUIComponent_AnimateFlocking>() },
            //{ BehaviorMethod.Animate_VectorField, animations.vectorField.GetComponent<GUIComponent_AnimateVectorField>() },
            //{ BehaviorMethod.Animate_WindSimulation, animations.windSimulation.GetComponent<GUIComponent_AnimateWindSimulation>() },
            { BehaviorMethod.Animate_StrangeAttractor, animations.strangeAttractor.GetComponent<GUIComponent_AnimateStrangeAttractor>() },
            //{ BehaviorMethod.Animate_LotkaVolterra, animations.lotkaVolterra.GetComponent<GUIComponent_AnimateLotkaVolterra>() },
            //{ BehaviorMethod.Animate_SpringSystem, animations.springSystem.GetComponent<GUIComponent_AnimateSpringSystem>() },
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
        };

        ComponentsBehavior = new Dictionary<BehaviorMethod, PointBehavior>()
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
        // GUI
        GameObject go = Instantiate(ComponentsPrefab[method], componentParentAnimation);
        GUIComponent compGUI = go.GetComponent<GUIComponent>();    // TODO: Do I need to downcast?
        if (compGUI == null)
        {
            Debug.LogWarning("GUIComponent missing from instantiated component. Destroying instantiated GameObject.");
            Destroy(go);
            return;
        }
        ConditionalPopulateDropdown(compGUI.transform);

        // Checking over all GUIController_Organization components and conditionally enabling/disabling repositioning buttons
        // TODO: Improve performance
        ConditionalSetRepositioningButtonsInteractive();
        
        // Functionality
        PointBehavior compBehavior = go.GetComponent<PointBehavior>();
        if (compBehavior == null)
        {
            Debug.LogWarning("PointBehavior missing from instantiated component. Destroying instantiated GameObject.");
            Destroy(go);
            return;
        }
        //compBehavior.ManagerPointSet = managerPointSet;
        managerPointSet.AddBehavior(compBehavior);
        // Force-updating GUI with newly instantiated component
        StartCoroutine(RebuildLayout_NextFrame(componentParentAnimation.GetComponent<RectTransform>()));
    }
    public void RemoveComponent(GameObject component)
    {
        // TODO: Make generic
        managerPointSet.RemoveBehavior(component.GetComponent<PointBehavior>());
        Destroy(component);
        // ...
        StartCoroutine(RebuildLayout_NextFrame(componentParentAnimation.GetComponent<RectTransform>()));
    }

    public void ApplyColorPalette(ColorPalette colorPalette)
    {
        IColorable.ApplyColorPalette_Image(menuBackgroundBorder, colorPalette.colorBackgroundPanel);
        IColorable.ApplyColorPalette_Image(menuBackgroundFill, colorPalette.colorBackgroundFill);
        // TODO: Scrollbar
        foreach (Transform component in componentParentAnimation)
        {
            component.GetComponent<IColorable>().ApplyColorPalette(colorPalette);
        }
    }

    public void PopulateDropdowns()
    {
        // TODO: Swap between 2D and 3D entries for all affected dropdowns (including AddComponent dropdown)
        componentSettings.Populate();
        componentGenerate.Populate();

        // Populates already-present interactable components
        foreach (Transform child in componentParentAnimation)
        {
            ConditionalPopulateDropdown(child);
        }

        componentAddComponent.Populate();
    }

    public static bool ConditionalPopulateDropdown(Transform target)
    {
        GUIComponent component = target.GetComponent<GUIComponent>();
        if (component == null)
            return false;

        IPopulatable populatable = component as IPopulatable;
        if (populatable == null)
            return false;

        populatable.Populate();
        return true;
    }

    public void ConditionalSetRepositioningButtonsInteractive()
    {
        foreach (Transform component in componentParentAnimation)
        {
            GUIController_Organization compOrganization = component.GetChild(0).GetChild(1).GetComponent<GUIController_Organization>();
            compOrganization.ConditionalSetButtonInteractive();
        }
    }
}
