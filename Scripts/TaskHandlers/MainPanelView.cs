using UnityEngine;

public class MainPanelView : MonoBehaviour
{
    [SerializeField] private TimerView _timerView;
    [SerializeField] private GameObject _addButton;

    private void OnEnable()
    {
        _timerView.Closed += OnTimerClosed;
    }

    private void OnDisable()
    {
        _timerView.Closed -= OnTimerClosed;
    }

    private void OnTimerClosed()
    {
        _addButton.SetActive(true);
    }
}
