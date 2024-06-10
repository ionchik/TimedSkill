using System.Linq;
using SimpleJSON;

public static class JsonParser
{
    public static SkillVariant ParseSkill(JSONNode skillNode)
    {
        string skillName = skillNode["Name"];
        StatType[] stats = skillNode["Stats"].AsStringList.Select(ParseStat).ToArray();
        return new SkillVariant(skillName, stats);
    }
    
    public static Skill ParseUserSkill(JSONNode skillNode, LevelHandler levelHandler)
    {
        string skillName = skillNode["Name"];
        int levelValue = skillNode["CurrentLevel"]["_value"].AsInt;
        int currentExperience = skillNode["CurrentLevel"]["_currentExperience"].AsInt;
        int neededExperience = skillNode["CurrentLevel"]["_neededExperience"].AsInt;
        StatType[] stats = skillNode["Stats"].AsStringList.Select(ParseStat).ToArray();
        Level level = new Level(levelValue, currentExperience, neededExperience, levelHandler);
        return new Skill(skillName, level, stats);
    }
    
    private static StatType ParseStat(string stringStat)
    {
        return (StatType)int.Parse(stringStat);
    }
}