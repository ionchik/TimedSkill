public class Time
{
    private const int SecondsInMinute = 60;
    
    private TimeValue _minutes;
    private TimeValue _seconds;

    public Time(int seconds)
    {
        _minutes = new TimeValue(seconds / SecondsInMinute);
        _seconds = new TimeValue(seconds % SecondsInMinute);
    }
    
    public Time(TimeValue minutes, TimeValue seconds)
    {
        _minutes = minutes ?? new TimeValue(0);
        _seconds = seconds ?? new TimeValue(0);
    }

    public int Value => _minutes.GetValue() * SecondsInMinute + _seconds.GetValue();
    public bool IsPositive => Value > 0;
    
    public override string ToString()
    {
        return _minutes + ":" + _seconds;
    }

    public void Decrement()
    {
        int newValue = Value - 1;
        _minutes = new TimeValue(newValue / SecondsInMinute);
        _seconds = new TimeValue(newValue % SecondsInMinute);
    }
}
