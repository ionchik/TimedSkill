using System;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{
    [SerializeField] private TaskForm _taskForm;
    [SerializeField] private TimerBehaviour _timerBehaviour;

    public event Action<TimedTask> TaskStarted;
    public event Action<TimedTask> TaskEnded;

    private TimedTask _currentTask;
    
    private void OnEnable()
    {
        _timerBehaviour.TimeEnded += OnTaskEnd;
        _taskForm.Submitted += CreateTask;
    }

    private void OnDisable()
    {
        _timerBehaviour.TimeEnded -= OnTaskEnd;
        _taskForm.Submitted -= CreateTask;
    }

    private void CreateTask(string title, string description, Time time)
    {
        _currentTask = new TimedTask(title, description, time);
        TaskStarted?.Invoke(_currentTask);
    }

    private void OnTaskEnd()
    {
        TaskEnded?.Invoke(_currentTask);
    }
}
