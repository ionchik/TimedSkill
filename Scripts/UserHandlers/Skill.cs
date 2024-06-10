using System;
using System.Linq;

public class Skill
{
    private readonly string _name;
    private readonly Level _level;
    private readonly StatType[] _stats;

    public Skill(SkillVariant skillVariant, LevelHandler levelHandler)
    {
        _name = skillVariant.Name;
        _stats = skillVariant.Stats;
        _level = new Level(levelHandler);
    }
    
    public Skill(string name, Level level, StatType[] stats)
    {
        _name = name;
        _stats = stats;
        _level = level;
    }

    public event Action<Level> Upgraded;
    public event Action Deleting;

    public SkillData GetData() => new SkillData(_name, _level, _stats);
    public SkillVariant GetVariant() => new SkillVariant(_name, _stats);
    public string GetName() => _name;
    public Level GetLevel() => _level;
    public StatType[] GetStats() => _stats;

    public void Delete()
    {
        Deleting?.Invoke();
    }
    public void AddExperience(int experience) 
    {
        _level.AddExperience(experience);
        Upgraded?.Invoke(_level);
    }

    public override string ToString()
    {
        string stats = _stats.Aggregate("; dependent stats: ", (current, statType) => current + (statType + ", "));
        return _name + "(" + _level + stats + ")";
    }
}
