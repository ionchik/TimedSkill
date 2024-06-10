public class TimeValue
{
    private const int Max = 59;

    private readonly int _value;

    public TimeValue(int value)
    {
        _value = value > Max ? Max : value;
    }

    public int GetValue() => _value;

    public override string ToString()
    {
        return  _value > 9 ? _value.ToString() : "0" + _value;
    }
}
