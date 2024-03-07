using System.Collections;
using UnityEngine;
using UnityEngine.UI;



[ExecuteAlways, RequireComponent(typeof(Toggle))]
public class GUIController_Toggle : MonoBehaviour
{
    [SerializeField] protected Toggle toggle;
    public Toggle Toggle { get { return toggle; } }
    [SerializeField] protected GameObject toggleGlyphOn;
    [SerializeField] protected GameObject toggleGlyphOff;



    protected void Awake()
    {
        if (!ConditionalFindReferences())
        {
            Debug.LogWarning("Vital references not found. Disabling this GameObject.");
            gameObject.SetActive(false);
            return;
        }

        SetToggleStateVisuals(Toggle.isOn);
    }

    protected void OnEnable()
    {
        toggle.onValueChanged.AddListener(SetToggleStateVisuals);
    }

    protected void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(SetToggleStateVisuals);
    }

    protected bool ConditionalFindReferences()
    {
        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
            if (toggle == null)
            {
                Debug.LogWarning("Toggle \"toggle\" is null. Failed operation.");
                return false;
            }
        }
        // Cannot assume location in hierarchy, as visual ordering may differ
        if (toggleGlyphOn == null)
        {
            Debug.LogWarning("GameObject \"toggleGlyphOn\" is null. Failed operation.");
            return false;
        }
        if (toggleGlyphOff == null)
        {
            Debug.LogWarning("GameObject \"toggleGlyphOff\" is null. Failed operation.");
            return false;
        }

        return true;
    }

    protected IEnumerator QueueSetToggleStateVisuals(bool isOn)
    {
        yield return new WaitForEndOfFrame();

        if (!ConditionalFindReferences())
        {
            Debug.LogWarning("Vital references not found. Cannot update toggle state visuals. Disabling this GameObject.");
            gameObject.SetActive(false);
            yield return null;
        }

        ToggleStateVisuals(isOn);

        yield return null;
    }

    protected virtual void ToggleStateVisuals(bool isOn)
    {
        toggleGlyphOn.SetActive(isOn);
        toggleGlyphOff.SetActive(!isOn);
    }

    public void SetToggleStateVisuals(bool isOn)
    {
        StartCoroutine(QueueSetToggleStateVisuals(isOn));
    }
}
