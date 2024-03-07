using UnityEngine;
using UnityEngine.UI;
using TMPro;



/*
    To do: 
    [ ] Bug: Value type = unsigned int, value function = power of two, value increments = 1, click-dragging the slider bar yields "infinity"
    [ ] Bug: Value type = unsigned int, value function = power of two, value increments = 1, typing in value other than 1 and 2 yields "infinity", also typing in 1 and 2 yields incorrect value (possibly evaluating value too many times?)
*/

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

public class GUIController_IncrementalSlider : MonoBehaviour
{
    [Header("Values")]
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
                    return valueMax;
            }
        }
        set
        {
            valueMax = value;
            valueSlider.maxValue = ValueMax;
            ReflectValueChange();
        }
    }
    [SerializeField] private float valueIncrements = 0.7853981f;
    [SerializeField, Tooltip("Allows text input values to remain. Does still not allow button increments / decrements to overflow the [valueMin, valueMax] interval.")]
    private bool allowInputOverflow = false;
    [SerializeField, Tooltip("Decimal places for text input.")]
    private uint decimalPlaces = 2;

    [Header("Unit Notation")]
    [SerializeField] private string textUnit = "s";
    [SerializeField] private Vector2 monospaceWidthInterval = new Vector2(5f, 9f);
    [SerializeField] private Vector2 monospaceWidthFontSizeInterval = new Vector2(8f, 16f);
    private float monospaceWidth = 8.8f;

    [Header("References")]
    [SerializeField] private Button buttonDecrement;
    [SerializeField] private Button buttonIncrement;
    [SerializeField] private Slider valueSlider;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text inputDisplayText;

    public Button ButtonDecrement { get { return buttonDecrement; } }
    public Button ButtonIncrement { get { return buttonIncrement; } }
    public Slider Slider { get { return valueSlider; } }
    public TMP_InputField InputField { get { return inputField; } }



    private void Awake()
    {
        if (!ConditionalFindReferences())
        {
            Debug.LogError("Could not find vital references. Disabling this GameObject.");
            gameObject.SetActive(false);
            return;
        }

        ReflectValueChange();
    }

    private void OnEnable()
    {
        if (!ConditionalFindReferences())
        {
            Debug.LogError("Could not find vital references. Disabling this GameObject.");
            gameObject.SetActive(false);
            return;
        }

        buttonDecrement.onClick.AddListener(DecrementValue);
        buttonIncrement.onClick.AddListener(IncrementValue);
        valueSlider.onValueChanged.AddListener(ReadValueFromSlider);
        inputField.onSubmit.AddListener(ReadValueFromTextInput);
    }

    private void OnDisable()
    {
        if (!ConditionalFindReferences())
        {
            Debug.LogError("Could not find vital references. Disabling this GameObject.");
            gameObject.SetActive(false);
            return;
        }

        buttonDecrement.onClick.RemoveListener(DecrementValue);
        buttonIncrement.onClick.RemoveListener(IncrementValue);
        valueSlider.onValueChanged.RemoveListener(ReadValueFromSlider);
        inputField.onSubmit.RemoveListener(ReadValueFromTextInput);
    }

    private void OnValidate()
    {
        if (!ConditionalFindReferences())
        {
            Debug.LogError("Could not find vital references. Disabling this GameObject.");
            gameObject.SetActive(false);
            return;
        }

        if (ValueMin > ValueMax)
        {
            Debug.LogWarning("valueMin must be less than or equal to valueMax. Setting valueMin equal to valueMax.");
            ValueMin = ValueMax;
        }

        valueSlider.minValue = ValueMin;
        valueSlider.maxValue = ValueMax;

        // Converting to integer if previously was decimal number
        if (valueType == ValueType.Int || valueType == ValueType.UnsignedInt)
        {
            sliderValue = Mathf.Clamp(Mathf.RoundToInt(sliderValue), ValueMin, ValueMax);
        }

        ReflectValueChange();
    }

    private bool ConditionalFindReferences()
    {
        // Failure here is catastrophic
        if (buttonDecrement == null)
        {
            buttonDecrement = transform.GetChild(0).GetComponent<Button>();
            if (buttonDecrement == null)
            {
                Debug.LogWarning("Could not find Button \"buttonDecrement\". Failed operation");
                return false;
            }
        }

        // Failure here is catastrophic
        if (buttonIncrement == null)
        {
            buttonIncrement = transform.GetChild(2).GetComponent<Button>();
            if (buttonIncrement == null)
            {
                Debug.LogWarning("Could not find Button \"buttonIncrement\". Failed operation");
                return false;
            }
        }

        // Failure here is catastrophic
        if (valueSlider == null)
        {
            valueSlider = transform.GetChild(1).GetChild(0).GetComponent<Slider>();
            if (valueSlider == null)
            {
                Debug.LogWarning("Could not find Slider \"valueSlider\". Failed operation");
                return false;
            }
        }

        // Failure here is acceptable
        if (inputField == null)
        {
            inputField = transform.GetChild(1).GetChild(1).GetComponent<TMP_InputField>();
            if (inputField == null)
            {
                Debug.LogWarning("Could not find TMP_InputField \"inputField\". Input field will not work. Continuing operation.");
            }
            else if (inputDisplayText == null)
            {
                inputDisplayText = inputField.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
                if (inputDisplayText == null)
                {
                    Debug.LogWarning("Could not find TMP_InputField \"inputField\". Input field will not display text. Continuing operation.");
                }
                else
                {
                    // TODO: Consider if we need this
                    monospaceWidth = 8.8f;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Evaluates float from text field input.
    /// </summary>
    /// <param name="inputValue">Value to attempt parsing into. Assigns value if successful, does nothing if unsuccessful. This is to guard against TryParse(), which sets the value to 0 if unsuccessful.</param>
    /// <returns>Whether the function was successful.</returns>
    private bool EvaluateFromTextInput(string inputString, ref float inputValue)
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
            // If integers only
            else
            {
                formattedString = Monospace(SliderValue.ToString(), monospaceWidth) + formattedUnit;
            }

            inputField.text = formattedString;
        }
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
