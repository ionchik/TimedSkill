using UnityEngine;

public class TimeInput : MonoBehaviour
{
    [SerializeField] private ValueInput _minutesField;
    [SerializeField] private ValueInput _secondsField;
    
    public Time Value => new Time(_minutesField.GetValue(), _secondsField.GetValue());

    public void Clear()
    {
        _secondsField.Clear();
        _minutesField.Clear();
    }
}
