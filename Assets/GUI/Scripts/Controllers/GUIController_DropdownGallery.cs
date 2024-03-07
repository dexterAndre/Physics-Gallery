using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIController_DropdownGallery : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button buttonDecrement;
    [SerializeField] private Button buttonIncrement;
    public TMP_Dropdown Dropdown { get { return dropdown; } }
    public Button ButtonDecrement { get { return buttonDecrement; } }
    public Button ButtonIncrement { get { return buttonIncrement; } }



    private void Awake()
    {
        if (!ConditionalFindReferences())
        {
            Debug.LogWarning("Vital references not found. Disabling this GameObject.");
            gameObject.SetActive(false);
            return;
        }
    }

    private void OnEnable()
    {
        buttonDecrement.onClick.AddListener(DecrementGallery);
        buttonIncrement.onClick.AddListener(IncrementGallery);
    }

    private void OnDisable()
    {
        buttonDecrement.onClick.RemoveListener(DecrementGallery);
        buttonIncrement.onClick.RemoveListener(IncrementGallery);
    }

    private bool ConditionalFindReferences()
    {
        if (buttonDecrement == null)
        {
            buttonDecrement = transform.GetChild(0).GetComponent<Button>();
            if (buttonDecrement == null)
            {
                Debug.LogWarning("Button \"buttonDecrement\" is null. Failed operation.");
                return false;
            }
        }
        if (dropdown == null)
        {
            dropdown = transform.GetChild(1).GetComponent<TMP_Dropdown>();
            if (dropdown == null)
            {
                Debug.LogWarning("TMP_Dropdown \"dropdown\" is null. Failed operation.");
                return false;
            }
        }
        if (buttonIncrement == null)
        {
            buttonIncrement = transform.GetChild(2).GetComponent<Button>();
            if (buttonIncrement == null)
            {
                Debug.LogWarning("Button \"buttonIncrement\" is null. Failed operation.");
                return false;
            }
        }

        return true;
    }

    public void DecrementGallery()
    {
        AdvanceGallery(-1);
    }

    public void IncrementGallery()
    {
        AdvanceGallery(1);
    }

    public int AdvanceGallery(int amount = 1)
    {
        // If option count is zero, cannot advance
        if (dropdown.options.Count <= 0)
        {
            Debug.LogWarning("Warning: Dropdown option count is 0. Could not advance gallery. Returning -1.");
            return -1;
        }

        // Loops around itself, all integer values are valid here
        dropdown.value = MathUtils.Wrap(dropdown.value + amount, 0, dropdown.options.Count);
        dropdown.RefreshShownValue();

        return dropdown.value;
    }
}
