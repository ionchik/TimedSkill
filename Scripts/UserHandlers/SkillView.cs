using UnityEngine;
using UnityEngine.UI;

public class SkillView : MonoBehaviour
{
    [SerializeField] private Text _levelView;
    [SerializeField] private Text _title;
    [SerializeField] private Image _progressFill;
    [SerializeField] private Text[] _statViews;

    private Skill _skill;
    
    public void SetSkill(Skill skill)
    {
        _skill = skill;
        _skill.Upgraded += Refresh;
        _skill.Deleting += OnDelete;
        Level level = skill.GetLevel();
        _title.text = skill.GetName();
        Refresh(level);

        StatType[] stats = skill.GetStats();

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

    private void Refresh(Level level)
    {
        _levelView.text = level.ToString();
        _progressFill.fillAmount = level.GetProgress();
    }

    private void OnDelete()
    {
        Destroy(gameObject);
    }
}
