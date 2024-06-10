using System;

[Serializable]
public class StatData : IData
{
   public StatType Stat;
   public Level CurrentLevel;

   public StatData(StatType stat, Level level)
   {
      Stat = stat;
      CurrentLevel = level;
   }
}
