using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    [SerializeField] private AnimationCurve _experiencePerLevel;
    [SerializeField] private int _maxExperience;
    [SerializeField] private int _maxLevel;

    public int GetLevelExperience(int level)
    {
        return (int)(_experiencePerLevel.Evaluate(level / (float)_maxLevel) * _maxExperience);
    }
}
