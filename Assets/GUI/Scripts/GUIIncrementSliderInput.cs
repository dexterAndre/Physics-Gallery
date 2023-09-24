using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Runtime.Serialization.Json;

public class GUIIncrementSliderInput : MonoBehaviour
{
    // Functionality
    [SerializeField] private float value = 6.283185f;
    [SerializeField] private float valueMin = 0.0f;
    [SerializeField] private float valueMax = 10.0f;
    [SerializeField] private float valueIncrements = 0.25f;
    [SerializeField, Tooltip("Allows text input values to remain. Does still not allow button increments / decrements to overflow the [valueMin, valueMax] interval.")]
    private bool allowInputOverflow = false;
    [SerializeField, Tooltip("Decimal places for text input. Set to -1 to disable.")]
    private int decimalPlaces = -1;
    [SerializeField] private Vector2 monospaceWidthInterval = new Vector2(5f, 9f);
    [SerializeField] private Vector2 monospaceWidthFontSizeInterval = new Vector2(8f, 16f);
    private float monospaceWidth = 8.8f;
    [SerializeField] private string textUnit = string.Empty;

    // References
    [SerializeField] private Transform parentPanel;
    [SerializeField] private Button buttonDecrement;
    [SerializeField] private Button buttonIncrement;
    [SerializeField] private Slider valueSlider;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text inputDisplayText;



    private void Awake()
    {
        if (parentPanel == null)
        {
            Transform currentParent = transform.parent;
            while (parentPanel == null && currentParent != null)
            {
                if (currentParent.GetComponent<GUIResizeEventListener>() != null)
                {
                    parentPanel = currentParent;
                }
                else
                {
                    currentParent = currentParent.parent;
                }
            }

            if (parentPanel == null)
            {
                Debug.LogWarning("Warning: parentPanel is not set. Will not respond to resize events.");
            }
        }

        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                //ReadValueFromTextInput(inputField.text);
            }
        }

        if (inputDisplayText == null)
        {
            inputDisplayText = transform.GetChild(1).GetChild(1).GetChild(0).Find("txt_inputText").GetComponent<TMP_Text>();
            if (inputDisplayText == null)
            {
                // Setting a fixed monospace width if text component not found
                monospaceWidth = 8.8f;
            }
        }

        if (valueSlider != null)
        {
            valueSlider.minValue = valueMin;
            valueSlider.maxValue = valueMax;
            if (inputField == null)
            {
                //ReadValueFromSlider(valueSlider.value);
            }
        }

        ReflectValueChange();
    }

    private void OnEnable()
    {
        if (parentPanel != null)
        {
            parentPanel.GetComponent<GUIResizeEventListener>().onResizeEvent += UpdateMonospaceWidth;
        }
        buttonDecrement?.onClick.AddListener(DecrementValue);
        buttonIncrement?.onClick.AddListener(IncrementValue);
        valueSlider?.onValueChanged.AddListener(ReadValueFromSlider);
        inputField?.onSubmit.AddListener(ReadValueFromTextInput);        
    }

    private void OnDisable()
    {
        if (parentPanel != null)
        {
            parentPanel.GetComponent<GUIResizeEventListener>().onResizeEvent -= UpdateMonospaceWidth;
        }
        buttonDecrement?.onClick.RemoveListener(DecrementValue);
        buttonIncrement?.onClick.RemoveListener(IncrementValue);
        valueSlider?.onValueChanged.RemoveListener(ReadValueFromSlider);
        inputField?.onSubmit.RemoveListener(ReadValueFromTextInput);
    }

    private void OnValidate()
    {
        if (valueMin > valueMax)
        {
            Debug.LogWarning("Warning: valueMin must be less than or equal to valueMax. Setting valueMin equal to valueMax");
            valueMin = valueMax;
        }

        valueSlider.minValue = valueMin;
        valueSlider.maxValue = valueMax;

        //// Defaults to favoring the text input field, selects slider as a fallback
        //if (!EvaluateFromTextInput(inputField.text, ref value))
        //{
        //    value = valueSlider.value;
        //}

        ReflectValueChange();
    }

    /// <summary>
    /// Evaluates float from text field input.
    /// </summary>
    /// <param name="inputValue">Value to attempt parsing into. Assigns value if successful, does nothing if unsuccessful. This is to guard against TryParse(), which sets the value to 0 if unsuccessful.</param>
    /// <returns>Whether the function was successful.</returns>
    public bool EvaluateFromTextInput(string inputString, ref float inputValue)
    {
        float parsedValue;
        if (float.TryParse(inputString, out parsedValue))
        {
            inputValue = parsedValue;
            return true;
        }

        return false;
    }

    private void SetValue(float newValue)
    {
        value = newValue;
        ReflectValueChange();
    }

    public void ReadValueFromTextInput(string inputString)
    {
        if (EvaluateFromTextInput(inputString, ref value))
        {
            if (!allowInputOverflow)
            {
                value = Mathf.Clamp(value, valueMin, valueMax);
            }
        }

        SetValue(value);
    }

    public void ReadValueFromSlider(float sliderValue)
    {
        // Snaps value to valueIncrements
        value = Mathf.Round(sliderValue / valueIncrements) * valueIncrements;
        SetValue(value);
    }

    public void DecrementValue()
    {
        float targetValue = value - valueIncrements;
        SetValue(targetValue < valueMin ? valueMin : targetValue);
    }

    public void IncrementValue()
    {
        float targetValue = value + valueIncrements;
        SetValue(targetValue > valueMax ? valueMax : targetValue);
    }

    private void ReflectValueChange()
    {
        // Optional slider GUI
        if (valueSlider != null)
        {
            if (value < valueMin || value > valueMax)
            {
                valueSlider.SetValueWithoutNotify(Mathf.Clamp(value, valueMin, valueMax));
            }

            valueSlider.SetValueWithoutNotify(value);
        }

        // Optional text overlay
        if (inputField != null)
        {
            string formattedString;
            string formattedUnit = textUnit.Length > 0 ? " " + textUnit : "";

            monospaceWidth = CalculateMonospaceWidth();

            // If allowing decimal places
            if (decimalPlaces >= 0)
            {
                string decimalStyle = "F0";
                decimalStyle = decimalStyle.Replace("0", decimalPlaces.ToString());
                formattedString = Monospace(value.ToString(decimalStyle), monospaceWidth) + formattedUnit;
            }
            // If integers only
            else
            {
                formattedString = Monospace(value.ToString(), monospaceWidth) + formattedUnit;
            }

            inputField.text = formattedString;
        }
    }

    private void UpdateMonospaceWidth()
    {
        monospaceWidth = CalculateMonospaceWidth();
        ReflectValueChange();
    }

    private float CalculateMonospaceWidth()
    {
        if (inputDisplayText == null)
            return monospaceWidth;

        float currentFontSize = inputDisplayText.fontSize;
        float calculatedWidth = monospaceWidthInterval.x + ((currentFontSize - monospaceWidthFontSizeInterval.x) / (monospaceWidthFontSizeInterval.y - monospaceWidthFontSizeInterval.x)) * (monospaceWidthInterval.y - monospaceWidthInterval.x);

        return Mathf.Clamp(calculatedWidth, monospaceWidthInterval.x, monospaceWidthInterval.y);
    }

    public static string Monospace(string stringInput, float spacing)
    {
        return $"<mspace={spacing.ToString()}px>{stringInput}</mspace>";
    }
}
