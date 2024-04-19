using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BehaviorSpecifications;



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
public struct PrefabCollection_Selection
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
public struct BehaviorData
{
    public string MethodName;
    public GameObject ComponentPrefab;
    public GUIComponent GUIComponent;
    public PointBehavior PointBehavior;


    public BehaviorData(string InMethodName, GameObject InComponentPrefab)
    {
        MethodName = InMethodName;
        ComponentPrefab = InComponentPrefab;
        GUIComponent = InComponentPrefab.GetComponent<GUIComponent>();
        PointBehavior = InComponentPrefab.GetComponent<PointBehavior>();
    }
    public BehaviorData(string InMethodName, GameObject InComponentPrefab, GUIComponent InGUIComponent, PointBehavior InPointBehavior)
    {
        MethodName = InMethodName;
        ComponentPrefab = InComponentPrefab;
        GUIComponent = InGUIComponent;
        PointBehavior = InPointBehavior;
    }
    public static List<string> BehaviorNames(Dictionary<BehaviorMethod, BehaviorData> InDictionary)
    {
        List<string> names = new List<string>();
        foreach (KeyValuePair<BehaviorMethod, BehaviorData> item in InDictionary)
        {
            names.Add(item.Value.MethodName);
        }

        return names;
    }
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
    [SerializeField] private Transform componentParentSelection;
    [SerializeField] private GUIComponent_AddComponent componentAddComponent;
    [SerializeField] private Manager_PointSet managerPointSet;

    [Header("Components")]
    [SerializeField] private PrefabCollection_Animation animations;
    [SerializeField] private PrefabCollection_Overlay overlays;
    [SerializeField] private PrefabCollection_Selection selections;
    // Initialize in OnEnable due to reading from inspector-populated structs
    private Dictionary<BehaviorMethod, BehaviorData> behaviors = new Dictionary<BehaviorMethod, BehaviorData>();
    public Dictionary<BehaviorMethod, BehaviorData> Behaviors { get { return behaviors; } }

    [Header("Dropdowns")]
    //[SerializeField] private GUIOption_DropdownGallery dropdownPresets;
    [SerializeField] private GUIOption_Dropdown dropdownBounds;
    [SerializeField] private GUIOption_Dropdown dropdownEdgeResponse;
    [SerializeField] private GUIOption_Dropdown dropdownGenerateMethod;



    private void OnEnable()
    {
        behaviors = new Dictionary<BehaviorMethod, BehaviorData>()
        {
            // Overlay
            //{ BehaviorMethod.Overlay_Web, new BehaviorData("Web", overlays.web) },
            //{ BehaviorMethod.Overlay_Triangulation, new BehaviorData("Triangulation", overlays.triangulation) },
            //{ BehaviorMethod.Overlay_ConvexHull, new BehaviorData("Convex Hull", overlays.convexHull) },
            //{ BehaviorMethod.Overlay_Voronoi, new BehaviorData("Voronoi Diagram", overlays.voronoi) },
            //{ BehaviorMethod.Overlay_Duals, new BehaviorData("Duals", overlays.duals) },
            //{ BehaviorMethod.Overlay_SpatialPartitioning, new BehaviorData("Spatial Partitioning", overlays.spatialPartitioning) },
            //{ BehaviorMethod.Overlay_CenterOfMass, new BehaviorData("Center of Mass", overlays.centerOfMass) },
            // Selection
            //{ BehaviorMethod.Selection_ClosestToRay, new BehaviorData("Closest to Ray", selections.closestToRay) },
            //{ BehaviorMethod.Selection_kMeansClustering, new BehaviorData("k-Means Clustering", selections.kMeansClustering) },
            //{ BehaviorMethod.Selection_PointSetRegistration, new BehaviorData("Point Set Registration", selections.pointSetRegistration) },
            // Animation
            { BehaviorMethod.Animation_Jitter, new BehaviorData("Jitter", animations.jitter) },
            //{ BehaviorMethod.Animation_Flocking, new BehaviorData("Flocking", animations.flocking) },
            //{ BehaviorMethod.Animation_VectorField, new BehaviorData("Vector Field", animations.vectorField) },
            //{ BehaviorMethod.Animation_WindSimulation, new BehaviorData("Wind Simulation", animations.windSimulation) },
            { BehaviorMethod.Animation_StrangeAttractor, new BehaviorData("Strange Attractor", animations.strangeAttractor) },
            //{ BehaviorMethod.Animation_LotkaVolterra, new BehaviorData("Lotka-Volterra Equations", animations.lotkaVolterra) },
            //{ BehaviorMethod.Animation_SpringSystem, new BehaviorData("Spring System", animations.springSystem) },
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
        GameObject go = Instantiate(Behaviors[method].ComponentPrefab, componentParentAnimation);
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
