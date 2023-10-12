using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Runtime.Serialization.Json;
using JetBrains.Annotations;

[System.Serializable]
public enum ValueType
{
    Float,
    Int,
    UnsignedInt
}

[System.Serializable]
public enum ValueFunction
{
    Linear,
    PowerOfTwo
}

public class GUIIncrementSliderInput : MonoBehaviour
{
    // Functionality
    /**
     * Value of the slider. 
     * Stored as a float, but if valueType is an integer type, all operations are int-based, 
     * and then this value can safely be typecast into integer values. 
     * Internally the value is stored as the input for the valueFunction, 
     * and only evaluated in the getter.
     * If you need to operate on the evaluated value, remember to take this into account.
     */
    [SerializeField] private float sliderValue = 6.283185f;
    public float SliderValue
    {
        get
        {
            switch (valueFunction)
            {
                case ValueFunction.Linear:
                    return sliderValue;
                case ValueFunction.PowerOfTwo:
                    return Mathf.Pow(2, sliderValue);
                default:
                    return sliderValue;
            }
        }
        set
        {
            sliderValue = value;
            ReflectValueChange();
        }
    }
    [SerializeField] private ValueType valueType = ValueType.Float;
    [SerializeField] private ValueFunction valueFunction = ValueFunction.Linear;
    [SerializeField] private float valueMin = 0.0f;
    public float ValueMin
    {
        get
        {
            switch (valueFunction)
            {
                case ValueFunction.Linear:
                    return valueMin;
                case ValueFunction.PowerOfTwo:
                    return Mathf.Pow(2, valueMin);
                default:
                    return valueMin;
            }
        }
        set
        {
            valueMin = value;
            valueSlider.minValue = ValueMin;
            ReflectValueChange();
        }
    }
    [SerializeField] private float valueMax = 10.0f;
    public float ValueMax
    {
        get
        {
            switch (valueFunction)
            {
                case ValueFunction.Linear:
                    return valueMax;
                case ValueFunction.PowerOfTwo:
                    return Mathf.Pow(2, valueMax);
                default:
                    return valueMin;
            }
        }
        set
        {
            valueMax = value;
            valueSlider.maxValue = ValueMax;
            ReflectValueChange();
        }
    }
    [SerializeField] private float valueIncrements = 1.0f;
    [SerializeField, Tooltip("Allows text input values to remain. Does still not allow button increments / decrements to overflow the [valueMin, valueMax] interval.")]
    private bool allowInputOverflow = false;
    [SerializeField, Tooltip("Decimal places for text input.")]
    private uint decimalPlaces = 0;
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
            valueSlider.minValue = ValueMin;
            valueSlider.maxValue = ValueMax;
            if (inputField == null)
            {
                // TODO: Consider removing
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
        if (ValueMin > ValueMax)
        {
            Debug.LogWarning("Warning: valueMin must be less than or equal to valueMax. Setting valueMin equal to valueMax");
            ValueMin = ValueMax;
        }

        valueSlider.minValue = ValueMin;
        valueSlider.maxValue = ValueMax;

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

    public void ReadValueFromTextInput(string inputString)
    {
        if (EvaluateFromTextInput(inputString, ref sliderValue))
        {
            bool castToInt = valueType == ValueType.Int || valueType == ValueType.UnsignedInt;

            if (!allowInputOverflow)
            {
                SliderValue = Mathf.Clamp(SliderValue, ValueMin, ValueMax);
                if (castToInt)
                    SliderValue = (int)SliderValue;
            }
        }

        //SliderValue = sliderValue;
    }

    public void ReadValueFromSlider(float value)
    {
        // Snaps value to valueIncrements
        SliderValue = Mathf.Round(value / valueIncrements) * valueIncrements;
    }

    public void DecrementValue()
    {
        // Directly setting the internal value to avoid evaluation errors
        float targetValue = sliderValue - valueIncrements;
        // Clamps on the minimum side, preventing clamping to the maximum side if allowing overflow
        sliderValue = targetValue < valueMin ? valueMin : targetValue;
        ReflectValueChange();
    }

    public void IncrementValue()
    {
        // Directly setting the internal value to avoid evaluation errors
        float targetValue = sliderValue + valueIncrements;
        // Clamps on the maximum side, preventing clamping to the minimum side if allowing overflow
        sliderValue = targetValue > valueMax ? valueMax : targetValue;
        ReflectValueChange();
    }

    private void ReflectValueChange()
    {
        // Optional slider GUI
        if (valueSlider != null)
        {
            valueSlider.SetValueWithoutNotify(Mathf.Clamp(SliderValue, ValueMin, ValueMax));
        }

        // Optional text overlay
        if (inputField != null)
        {
            string formattedString;
            string formattedUnit = textUnit.Length > 0 ? " " + textUnit : "";

            monospaceWidth = CalculateMonospaceWidth();

            // If allowing decimal places
            if (decimalPlaces > 0)
            {
                string decimalStyle = "F0";
                decimalStyle = decimalStyle.Replace("0", decimalPlaces.ToString());
                formattedString = Monospace(SliderValue.ToString(decimalStyle), monospaceWidth) + formattedUnit;
            }
            // TODO: Consider if necessary
            // If integers only
            else
            {
                formattedString = Monospace(SliderValue.ToString(), monospaceWidth) + formattedUnit;
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
