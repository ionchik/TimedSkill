using System;
using UnityEngine;

[Serializable]
public class Level
{
    [SerializeField] private int _value;
    [SerializeField] private int _currentExperience;
    [SerializeField] private int _neededExperience;
    
    private LevelHandler _levelHandler;
    
    public Level(LevelHandler levelHandler)
    {
        _value = 1;
        _currentExperience = 0;
        _levelHandler = levelHandler;
        _neededExperience = _levelHandler.GetLevelExperience(_value);
    }

    public Level(int value, int currentExperience, int neededExperience, LevelHandler levelHandler)
    {
        _value = value;
        _currentExperience = currentExperience;
        _levelHandler = levelHandler;
        _neededExperience = neededExperience;
    }

    public int GetValue() => _value;
    public float GetProgress() => (float)_currentExperience / _neededExperience;
    
    public void AddExperience(int experience)
    {
        if (experience <= 0) throw new ArgumentException("Added EXP must be more than ZERO!");
        _currentExperience += experience;
        if (_currentExperience >= _neededExperience) Upgrade();
    }

    public void SetLevelHandler(LevelHandler levelHandler)
    {
        _levelHandler = levelHandler;
    }
    
    private void Upgrade()
    {
        _value++;
        _currentExperience -= _neededExperience;
        _neededExperience = _levelHandler.GetLevelExperience(_value);
    }

    public override string ToString()
    {
        return "Lvl. " + _value;
    }
}
