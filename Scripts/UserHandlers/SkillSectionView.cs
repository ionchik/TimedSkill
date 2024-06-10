using UnityEngine;

public class SkillSectionView : MonoBehaviour
{
    [SerializeField] private User _user;
    [SerializeField] private Transform _skillContainer;
    [SerializeField] private SkillView _skillView;

    private void OnEnable()
    {
        _user.SkillAdded += OnSkillAdd;
    }

    private void OnDisable()
    {
        _user.SkillAdded -= OnSkillAdd;
    }

    private void OnSkillAdd(Skill skill)
    {
        SkillView skillView = Instantiate(_skillView, _skillContainer);
        skillView.SetSkill(skill);
    }
}
