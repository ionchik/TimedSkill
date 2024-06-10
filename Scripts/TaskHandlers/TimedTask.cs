using System;
using UnityEngine;

[Serializable]
public class TimedTask
{
    [SerializeField] private readonly string _title;
    [SerializeField] private readonly string _description;
    [SerializeField] private readonly Time _time;

    public TimedTask(string title, string description, Time time)
    {
        _title = title;
        _description = description;
        _time = time;
    }

    public string GetTitle() => _title;
    public string GetDescription() => _description;
    public Time GetTime() => _time;
}
