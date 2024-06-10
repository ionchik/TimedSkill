using UnityEngine;
using UnityEngine.UI;

public class StatView : MonoBehaviour
{
    [SerializeField] private Stat _stat;
    [SerializeField] private Text _levelView;
    [SerializeField] private Image _levelFill;

    private void OnEnable()
    {
        _stat.Updated += Refresh;
    }

    private void OnDisable()
    {
        _stat.Updated -= Refresh;
    }

    private void Refresh(Level level)
    {
        _levelView.text = level.ToString();
        _levelFill.fillAmount = level.GetProgress();
    }
}
