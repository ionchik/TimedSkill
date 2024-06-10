using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TimerBehaviour _timerBehaviour;
    [SerializeField] private TaskHandler _taskHandler;
    [SerializeField] private Text _timeIndicator;
    [SerializeField] private Text _taskTitle;
    [SerializeField] private Image _timerFiller;
    [SerializeField] private GameObject _timer;

    public event Action Closed;
    
    private void OnEnable()
    {
        _timerBehaviour.TimeChanged += Refresh;
        _timerBehaviour.TimeEnded += CloseTimer;
        _timerBehaviour.TimeStopped += CloseTimer;
        _taskHandler.TaskStarted += InitView;
    }

    private void OnDisable()
    {
        _timerBehaviour.TimeChanged -= Refresh;
        _timerBehaviour.TimeEnded -= CloseTimer;
        _timerBehaviour.TimeStopped -= CloseTimer;
        _taskHandler.TaskStarted -= InitView;
    }

    private void InitView(TimedTask timedTask)
    {
        Time time = timedTask.GetTime();
        _timeIndicator.text = time.ToString();
        _timerFiller.fillAmount = 1;
        _taskTitle.text = timedTask.GetTitle();
    }

    private void Refresh(Time current, int initialTime)
    {
        _timeIndicator.text = current.ToString();
        _timerFiller.fillAmount = (float)current.Value / initialTime;
    }

    private void CloseTimer()
    {
        _timer.SetActive(false);
        Closed?.Invoke();
    } 
}
