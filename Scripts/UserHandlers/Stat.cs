using System;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField] private LevelHandler _levelHandler;
    [SerializeField] private StatType _type; 
    
    private Level _level;

    public event Action<Level> Updated; 
    
    private void Start()
    {
        if (_level != null) return; 
        _level = new Level(_levelHandler);
        Updated?.Invoke(_level);
    }

    public StatData GetData() => new StatData(_type, _level);
    public StatType GetStatType() => _type;

    public void AddExperience(int experience) 
    {
        _level.AddExperience(experience);
        Updated?.Invoke(_level);
    }

    public void SetLevel(Level level)
    {
        _level = level;
        _level.SetLevelHandler(_levelHandler);
        Updated?.Invoke(_level);
    }
}
