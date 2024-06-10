using System;

[Serializable]
public class UserData : IData
{
    public StatData[] Stats;
    public SkillData[] Skills;

    public UserData(StatData[] stats, SkillData[] skills)
    {
        Stats = stats;
        Skills = skills;
    }
}