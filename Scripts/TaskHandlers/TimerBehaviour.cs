using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimerBehaviour : MonoBehaviour
{
    [SerializeField] private TaskHandler _taskHandler;
    [SerializeField] private Button _closeButton;
    
    public event Action TimeEnded;
    public event Action TimeStopped;
    public event Action<Time, int> TimeChanged;

    private IEnumerator _currentTimer;

    private void OnEnable()
    {
        _taskHandler.TaskStarted += OnTaskStart;
        _closeButton.onClick.AddListener(StopTimer);
    }

    private void OnDisable()
    {
        _taskHandler.TaskStarted -= OnTaskStart;
        _closeButton.onClick.RemoveListener(StopTimer);
    }

    private void StopTimer()
    {
        StopCoroutine(_currentTimer);
        TimeStopped?.Invoke();
    }

    private void OnTaskStart(TimedTask timedTask)
    {
        _currentTimer = Timer(new Time(timedTask.GetTime().Value));
        StartCoroutine(_currentTimer);
    }

    private IEnumerator Timer(Time time)
    {
        int initTime = time.Value;
        WaitForSecondsRealtime second = new WaitForSecondsRealtime(1);
        
        while (time.IsPositive)
        {
            yield return second;
            time.Decrement();
            TimeChanged?.Invoke(time, initTime);
        } 
        
        TimeEnded?.Invoke();
    }
}
