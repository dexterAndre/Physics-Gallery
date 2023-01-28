using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


/*
################# Legend #################

[ ]     Tickbox (non-exclusive)
( )     Radio button (exclusive)
 V      Dropdown (text indicates label)
[V]     Dropdown item
-o-     Slider (always comes with editable input field)

def:    Default value indicator
 *      Contextual GUI element
[a, b]  Clamping (percentage unless otherwise specified)

##########################################
########## Contextual Settings: ##########
##########################################

################ Generate ################

 V  "Generate using" def Random
    [V] "Random"
        [ ] "Inside Radius" def false
            - Tooltip: "Generates random points inside a specified radius."
            * -o- "Radius" [0, 50%] def: 50%
                - Tooltip: "Radius to generate points inside."
        [ ] "Disk Sampling" def false
            - Tooltip: "Generates random points within a specified distance from each other."
            * -o- "Disk Radius" [0, 50%] def: 2.5%
                - Tooltip: "Radius of free space between generated points."
            * -o- "Generation Attempts" [1, 20] def: 3
                - Tooltip: "How many times to attempt to spawn if blocked by another disk."
            * -o- "Termination Condition" [1, 20] def: 10
                - Tooltip: "How many times allowed to fail before halting generation altogether."
    [V] "Lattice"
        -o- "Density" [0, 1 000 000] (counts elements) def: 100
            - Tooltip: "How many points inside
         V  "Bias" def: Linear
            - Tooltip: "Probability bias for shifting points upon generation. All functions are normalized within [-1, 1]."
            [V] "Linear"
                - Tooltip: "No bias. 
            [V] "Normal Distribution"
            [V] "Parabola"
         V  "Type"
            [V] "Square"
            [V] "Hexagonal"
    [V] "Import"
        [ ] "Normalize" def true
            - Tooltip: "Normalizes the data set within [-1, 1] in all three dimensions."
            * -o- "Scale" [0, 200%] def: 100%
                - Tooltip: "Scale for imported data set after normalization."
 V  "in"
    [V] "2D"
        - Tooltip: "Places generated points on a plane. Some algorithms behave differently in 2D."
    [V] "3D"
        - Tooltip: "Places generated points in 3D space. Some algorithms behave differently in 3D."

################ Overlay #################

################ Animate #################

 V  "Animate using" def Jitter
    [V] "Jitter"
        [ ] "
*/

public enum PointSetGenerateType
{
    None = -1,
    Random,
    Lattice,
    Import,
}
public enum PointSetOverlayType
{
    None = -1,
    Triangulation,
    N2Web,
    ConvexHull,
    VoronoiDiagram,
    Duals,
    kMeansClustering,
    ClosestPointToRay,
    Trace
}
public enum PointSetAnimateType
{
    None = -1,
    Jitter,
    LifeAndDeath,
    SpringSimulation,
    Flocking,
    VectorField,
    WindSimulation,
    StrangeAttractor,
    LotkaVolterraEquations,
}

public class ManagerGUI : MonoBehaviour
{
    private delegate IEnumerator CoroutineFunction();
    private static event CoroutineFunction TopBarTransition;

    List<string> PointSetDropdownChoicesGenerate = new List<string>
    {
        "Random",
        "Lattice",
        "Import",
    };
    List<string> PointSetDropdownChoicesOverlay = new List<string> 
    {
        "Triangulation",
        "n^2 Web",
        "Convex Hull",
        "Voronoi Diagram",
        "Duals",
        "k-Means Clustering",
        "Closest Point Selector",
        "Trace",
    };
    List<string> PointSetDropdownChoicesAnimate = new List<string>
    {
        "Jitter",
        "Life and Death",
        "Spring Simulation",
        "Flocking",
        "Vector Field",
        "Wind Simulation",
        "Strange Attractor",
        "Lotka-Volterra Equations",
    };

    [Header("Transitions")]
    [SerializeField] private float transitionDuration = 0.2f;
    [SerializeField] private AnimationCurve animTopBarCollapse;
    [SerializeField] private AnimationCurve animTopBarExpand;
    [SerializeField] private AnimationCurve animTopBarTransition;
    public bool IsTopBarFoldedOut { get { return GrpTopBarFoldOutHeight > 0; } }
    private int grpTopBarFoldOutHeightInit;
    public int GrpTopBarFoldOutHeight
    {
        get
        {
            return (int)grpTopBar.layout.size.y;
        }
        set
        {
            if (isFirstFrame)
            {
                Debug.LogWarning("Warning: Trying to set grp_TopBar height on first frame. Could return uninitialized values. Did not set height.");
                return;
            }

            grpTopBar.style.height = value;
        }
    }

    [Header("References")]
    [SerializeField] private GroupBox grpContents;
    [SerializeField] private GroupBox grpTopBar;
    [SerializeField] private GroupBox grpNavBar;
    [SerializeField] private GroupBox grpSettings;
    [SerializeField] private GroupBox grpSeparator;
    [SerializeField] private Button buttonFoldout;
    [SerializeField] private ManagerPointSet managerPointSet;
    [SerializeField] private VisualElement rootElement;
    [SerializeField] private DropdownField dropdownFieldPointSetGenerate;
    [SerializeField] private DropdownField dropdownFieldPointSetOverlay;
    [SerializeField] private DropdownField dropdownFieldPointSetAnimate;
    [SerializeField] private Button buttonNavBarBack;
    [SerializeField] private Button buttonNavBarMore;
    [SerializeField] private Button buttonPointSetGenerate;
    [SerializeField] private Toggle togglePointSetOverlay;
    [SerializeField] private Toggle togglePointSetAnimate;
    [SerializeField] private RadioButtonGroup togglePointSetDimension;

    private bool isFirstFrame = true;
    private bool isTopBarOccupied = false;
    


    private void Awake()
    {
        // References
        managerPointSet = GameObject.Find("Point Set Manager").GetComponent<ManagerPointSet>();
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        grpContents = rootElement.Q<GroupBox>("grp_Contents");
        grpTopBar = rootElement.Q<GroupBox>("grp_TopBar");
        grpNavBar = rootElement.Q<GroupBox>("grp_NavBar");
        grpSettings = rootElement.Q<GroupBox>("grp_Settings");
        grpSeparator = rootElement.Q<GroupBox>("grp_Separator");
        buttonFoldout = rootElement.Q<Button>("btn_Foldout");
        dropdownFieldPointSetGenerate = rootElement.Q<DropdownField>("drp_Generate");
        dropdownFieldPointSetOverlay = rootElement.Q<DropdownField>("drp_Overlay");
        dropdownFieldPointSetAnimate = rootElement.Q<DropdownField>("drp_Animate");
        buttonNavBarBack = rootElement.Q<Button>("btn_Back");
        buttonNavBarMore = rootElement.Q<Button>("btn_More");
        buttonPointSetGenerate = rootElement.Q<Button>("btn_Generate");
        togglePointSetOverlay = rootElement.Q<Toggle>("tgl_Overlay");
        togglePointSetAnimate = rootElement.Q<Toggle>("tgl_Animate");
        togglePointSetDimension = rootElement.Q<RadioButtonGroup>("tgl_Dimension");

        // Ensure all references are caught
        if (grpContents == null ||
            grpTopBar == null ||
            grpNavBar == null ||
            grpSettings == null ||
            grpSeparator == null ||
            buttonFoldout == null ||
            managerPointSet == null ||
            rootElement == null ||
            dropdownFieldPointSetGenerate == null ||
            dropdownFieldPointSetOverlay == null ||
            dropdownFieldPointSetAnimate == null ||
            togglePointSetDimension == null ||
            buttonNavBarBack == null ||
            buttonNavBarMore == null ||
            buttonPointSetGenerate == null ||
            togglePointSetOverlay == null ||
            togglePointSetAnimate == null)
        {
            gameObject.SetActive(false);
        }

        // Hooking up GUI elements
        dropdownFieldPointSetGenerate.choices = PointSetDropdownChoicesGenerate;
        dropdownFieldPointSetOverlay.choices = PointSetDropdownChoicesOverlay;
        dropdownFieldPointSetAnimate.choices = PointSetDropdownChoicesAnimate;

        buttonFoldout.clicked += ButtonFoldout_clicked;
        buttonNavBarBack.clicked += ButtonNavBarBack_clicked;
        buttonNavBarMore.clicked += ButtonNavBarMore_clicked;
        buttonPointSetGenerate.clicked += ButtonPointSetGenerate_clicked;

        togglePointSetOverlay.RegisterValueChangedCallback(TogglePointSetOverlay_clicked);
        togglePointSetAnimate.RegisterValueChangedCallback(TogglePointSetAnimate_clicked);
        togglePointSetDimension.RegisterValueChangedCallback(TogglePointSetDimension_clicked);

        // Settings for ManagerGUI (happens after first frame, so don't use any GUI values until isFirstFrame is false!)
        StartCoroutine(ReadInitGUIValues());

        // Settings for ManagerPointSet
        managerPointSet.Is2D = togglePointSetDimension.value == 0 ? true : false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator ReadInitGUIValues()
    {
        // Don't know when exactly the GUI values are initialized, so this is a hack to just wait one frame until everything is initialized
        yield return new WaitForEndOfFrame();

        isFirstFrame = false;

        // Reading initial values
        grpTopBarFoldOutHeightInit = GrpTopBarFoldOutHeight;
        SelectTopBarTransitionAnimation();
    }

    private IEnumerator TransitionTopBar(AnimationCurve curve)
    {
        if (isTopBarOccupied)
        {
            Debug.LogWarning("Warning: grp_TopBar is currently being modified by another coroutine. Stopping this coroutine call for TransitionTopBar().");
            CallbackTopBarTransition(false);
            yield break;
        }

        if (curve == null || curve.length <= 0f)
        {
            Debug.LogWarning("Warning: curve is invalid. Cannot read AnimationCurve value. Stopping this coroutine call for TransitionTopBar().");
            CallbackTopBarTransition(false);
            yield break;
        }

        float timer = 0f;
        while (timer < transitionDuration)
        {
            GrpTopBarFoldOutHeight = (int)(EvaluateCurveNormalized(animTopBarTransition, timer, transitionDuration) * grpTopBarFoldOutHeightInit);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        CallbackTopBarTransition(true);
    }

    private void OnTopBarTransitionFinished()
    {
        // Swap image
        // 
        // Set up animation
        SelectTopBarTransitionAnimation();
    }

    private void SwapTopBarImage()
    {
        // if isFoldedOut, one img, else another
    }

    private void SelectTopBarTransitionAnimation()
    {
        animTopBarTransition = IsTopBarFoldedOut ? animTopBarCollapse : animTopBarExpand;
    }

    private void CallbackTopBarTransition(bool completed)
    {
        isTopBarOccupied = !completed;
        if (completed)
            OnTopBarTransitionFinished();
    }

    private float EvaluateCurveNormalized(AnimationCurve curve, float t, float duration)
    {
        return curve.Evaluate(t / duration);
    }


    #region GUI Events
    private void ButtonFoldout_clicked()
    {
        StartCoroutine(TransitionTopBar(animTopBarTransition));
    }

    private void ButtonNavBarBack_clicked()
    {
        throw new System.NotImplementedException();
    }

    private void ButtonNavBarMore_clicked()
    {
        throw new System.NotImplementedException();
    }

    private void ButtonPointSetGenerate_clicked()
    {
        if (managerPointSet)
        {
            StartCoroutine(managerPointSet.GeneratePoints());
        }
    }

    private void TogglePointSetOverlay_clicked(ChangeEvent<bool> evt)
    {
        Debug.Log("Toggle clicked!");
    }

    private void TogglePointSetAnimate_clicked(ChangeEvent<bool> evt)
    {
        managerPointSet.AnimateJitter = evt.newValue;
    }
    
    private void TogglePointSetDimension_clicked(ChangeEvent<int> evt)
    {
        managerPointSet.Is2D = evt.newValue == 0 ? true : false;
    }
    #endregion
}
