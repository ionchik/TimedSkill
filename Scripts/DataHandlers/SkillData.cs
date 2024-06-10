using System;

[Serializable]
public class SkillData : IData
{
    public string Name;
    public Level CurrentLevel;
    public StatType[] Stats;

    public SkillData(string name, Level level, StatType[] stats)
    {
        Name = name;
        CurrentLevel = level;
        Stats = stats;
    }
}