using System;
using UnityEngine;
using UnityEngine.UI;

public class TaskForm : MonoBehaviour
{
    [SerializeField] private Button _finishButton;
    [SerializeField] private TimeInput _timeInput;
    [SerializeField] private InputField _titleInput;
    [SerializeField] private InputField _descriptionField;

    public event Action<string, string, Time> Submitted;

    private void OnEnable()
    {
        _finishButton.onClick.AddListener(OnFormFinished);
    }

    private void OnDisable()
    {
        _finishButton.onClick.RemoveListener(OnFormFinished);
    }

    private void OnFormFinished()
    {
        string title = _titleInput.text;
        if (title.Length == 0) return;
        
        Time time = _timeInput.Value;

        if (time.IsPositive)
        {
            string description = _descriptionField.text;
            Submitted?.Invoke(title, description, time);
            _titleInput.text = "";
            _descriptionField.text = "";
            _timeInput.Clear();
        }
    }
}
