using UnityEngine;
using UnityEngine.UI;

public class SkillComparisonView : MonoBehaviour
{
    [SerializeField] private Text _userLevelView;
    [SerializeField] private Text _opponentLevelView;
    [SerializeField] private Text _title;
    [SerializeField] private Image _progressFill;
    [SerializeField] private Text[] _statViews;
    
    public void SetSkill(SkillData skill, Skill opponentSkill)
    {
        _title.text = skill.Name;
        
        Level userLevel = skill.CurrentLevel;
        _userLevelView.text = userLevel.ToString();
        
        Level opponentLevel = opponentSkill.GetLevel();
        _opponentLevelView.text = opponentLevel.ToString();
        
        _progressFill.fillAmount = (float)userLevel.GetValue() / (opponentLevel.GetValue() + userLevel.GetValue());
        
        StatType[] stats = skill.Stats;

        for (int index = 0; index < _statViews.Length; index++)
        {
            if (index < stats.Length)
            {
                _statViews[index].gameObject.SetActive(true);
                _statViews[index].text = stats[index].ToString();
            }
            else
            {
                _statViews[index].gameObject.SetActive(false);
            }
        }
    }
}
