using UnityEngine;
using UnityEngine.UI;

public class ValueInput : MonoBehaviour
{
    [SerializeField] private InputField _selfInputField;
    [SerializeField] private InputField _otherInputField;
    
    private const int MaxLength = 2;
    
    private TimeValue _value;

    public TimeValue GetValue() => _value;
    
    private void OnEnable()
    {
        _selfInputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        _selfInputField.onValueChanged.RemoveListener(OnValueChanged);
    }

    public void Clear()
    {
        _selfInputField.text = "";
    }

    private void OnValueChanged(string input)
    {
        _value = new TimeValue(int.Parse(input));
        
        if (input.Length != MaxLength) return;
        
        _selfInputField.text = _value.ToString();
        if (_otherInputField.text.Length == 0) _otherInputField.Select();
    }
}
