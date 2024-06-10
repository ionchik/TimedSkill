using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleView : MonoBehaviour
{
    [SerializeField] private User _user;
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private Transform _skillContainer;
    [SerializeField] private SkillView _skillView;
    [SerializeField] private SkillComparisonView _skillComparisonView;
    [SerializeField] private ComparisonStatView[] _statViews;
    [SerializeField] private GameObject _battleScreen;

    public event Action<bool> BattleEnd;  
    
    private void OnEnable()
    {
        _battleManager.OpponentSkillsGot += RefreshSkills;
        _battleManager.OpponentStatsGot += RefreshStats;
    }

    private void OnDisable()
    {
        _battleManager.OpponentSkillsGot -= RefreshSkills;
        _battleManager.OpponentStatsGot -= RefreshStats;
    }
    
    private void RefreshStats(List<StatData> stats)
    {
        int wins = 0;
        StatData[] userStats = _user.GetData().Stats;
        for (int index = 0; index < userStats.Length; index++)
        {
            _statViews[index].SetInfo(userStats[index].CurrentLevel, stats[index].CurrentLevel);
            if (userStats[index].CurrentLevel.GetValue() > stats[index].CurrentLevel.GetValue())
            {
                wins++;
            }
        }
        BattleEnd?.Invoke(wins > 2);
        _battleScreen.SetActive(true);
    }

    private void RefreshSkills(List<Skill> skills)
    {
        foreach(Transform child in _skillContainer) {
            Destroy(child.gameObject);
        }
        
        UserData userData = _user.GetData();

        foreach (Skill opponentSkill in skills)
        {
            bool isSuitable = false;
            foreach (SkillData userSkill in userData.Skills)
            {
                if (userSkill.Name == opponentSkill.GetName())
                {
                    SkillComparisonView skillView = Instantiate(_skillComparisonView, _skillContainer);
                    skillView.SetSkill(userSkill, opponentSkill);
                    isSuitable = true;
                }
            }

            if (isSuitable == false)
            {
                SkillView skillView = Instantiate(_skillView, _skillContainer);
                skillView.SetSkill(opponentSkill);
            }
        }
    }
}
