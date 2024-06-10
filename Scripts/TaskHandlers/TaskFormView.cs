using System;
using Unity.VisualScripting;
using UnityEngine;

public class TaskFormView : MonoBehaviour
{
    [SerializeField] private TaskForm _taskForm;
    [SerializeField] private GameObject _timer;
    [SerializeField] private GameObject _descriptionState;
    [SerializeField] private GameObject _timeState;

    private void OnEnable()
    {
        _taskForm.Submitted += OnSubmitted;
    }

    private void OnDisable()
    {
        _taskForm.Submitted -= OnSubmitted;
    }

    private void OnSubmitted(string title, string description, Time time)
    {
        _timer.SetActive(true);
        _descriptionState.SetActive(false);
        _timeState.SetActive(true);
        gameObject.SetActive(false);
    }
}
