using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField] private SkillSystem _skillSystem;
    [SerializeField] private DatabaseBridge _databaseBridge;
    [SerializeField] private Stat[] _stats;

    public event Action<Skill> SkillAdded; 
    public event Action<SkillVariant, Level> ExperienceEarned; 
    public event Action<SkillVariant, Skill> SkillEvoluted; 
    
    private List<Skill> _skills;
    private string _id;

    private void Awake()
    {
        _id = SystemInfo.deviceUniqueIdentifier;
        _skills = new List<Skill>();
    }

    private void OnEnable()
    {
        _databaseBridge.SkillCreated += AddSkill;
        _skillSystem.ExperienceEarned += AddSkillExperience;
        _skillSystem.SkillEvolution += EvoluteSkill;
        _databaseBridge.SkillsInitialized += OnSkillsInit;
        _databaseBridge.StatsInitialized += OnStatsInit;
        _databaseBridge.SkillsMerged += OnSkillMerged;
    }

    private void OnDisable()
    {
        _databaseBridge.SkillCreated -= AddSkill;
        _skillSystem.ExperienceEarned -= AddSkillExperience;
        _skillSystem.SkillEvolution -= EvoluteSkill;
        _databaseBridge.SkillsInitialized -= OnSkillsInit;
        _databaseBridge.StatsInitialized -= OnStatsInit;
        _databaseBridge.SkillsMerged -= OnSkillMerged;
    }

    public string GetID() => _id;
    
    public UserData GetData()
    {
        StatData[] statsData = _stats.Select(s => s.GetData()).ToArray();
        SkillData[] skillsData = _skills.Select(s => s.GetData()).ToArray();
        return new UserData(statsData, skillsData);
    }
    
    public string GetSkillPrompt()
    {
        return _skills.Count == 0 ? "I have no skills to upgrade." : _skills.Aggregate("I have these skills: ", (current, skill) => current + (skill + ", "));
    }

    private void OnSkillMerged(Skill skill, List<SkillVariant> mergedSkills)
    {
        foreach (SkillVariant mergedSkill in mergedSkills)
        {
            DeleteSkill(mergedSkill.Name);
        }
        AddSkill(skill);
    }
    
    private void EvoluteSkill(SkillVariant oldVariant, Skill newSkill)
    {
        DeleteSkill(oldVariant.Name);
        AddSkill(newSkill);
        SkillEvoluted?.Invoke(oldVariant, newSkill);
    }

    private void DeleteSkill(string skillName)
    {
        foreach (Skill skill in _skills.Where(skill => skill.GetName() == skillName).ToList())
        {
            skill.Delete();
            _skills.Remove(skill);
        }
    }

    private void OnSkillsInit(List<Skill> skills)
    {
        foreach (Skill skill in skills)
        {
            AddSkill(skill);
        }
    }
    
    private void OnStatsInit(List<StatData> stats)
    {
        if (stats.Count == 0) return;
        foreach (StatData stat in stats)
        {
            _stats[(int)stat.Stat].SetLevel(stat.CurrentLevel);
        }
    }

    private void AddSkill(Skill skill)
    {
        AddStatExperience(skill, 15);
        _skills.Add(skill);
        
        SkillAdded?.Invoke(skill);
    }

    private void AddSkillExperience(SkillVariant skillVariant, int experience)
    {
        Skill skill = _skills.Find(skill => skill.GetName() == skillVariant.Name);
        AddStatExperience(skill, 5);
        skill.AddExperience(experience);
        ExperienceEarned?.Invoke(skillVariant, skill.GetLevel());
    }

    private void AddStatExperience(Skill skill, int experience)
    {
        foreach (StatType statType in skill.GetStats())
            _stats[(int) statType].AddExperience(experience);
    }
}
