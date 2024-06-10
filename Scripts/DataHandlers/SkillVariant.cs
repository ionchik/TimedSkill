using System;

[Serializable]
public class SkillVariant : IData
{
    public string Name;
    public StatType[] Stats;
    
    public SkillVariant(string name, StatType[] stats)
    {
        Name = name;
        Stats = stats;
    }
}
