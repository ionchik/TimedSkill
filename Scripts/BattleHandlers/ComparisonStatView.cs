using UnityEngine;
using UnityEngine.UI;

public class ComparisonStatView : MonoBehaviour
{
    [SerializeField] private Text _userLevelView;
    [SerializeField] private Text _opponentLevelView;
    [SerializeField] private Image _levelFill;

    public void SetInfo(Level userLevel, Level opponentLevel)
    {
        _userLevelView.text = userLevel.ToString();
        _opponentLevelView.text = opponentLevel.ToString();
        _levelFill.fillAmount = (float)userLevel.GetValue() / (userLevel.GetValue() + opponentLevel.GetValue());
    }
}
