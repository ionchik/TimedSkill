using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using SimpleJSON;
using UnityEngine;

public class DatabaseBridge : MonoBehaviour
{
    [SerializeField] private User _user;
    [SerializeField] private SkillSystem _skillSystem;
    [SerializeField] private LevelHandler _skillsLevelHandler;

    public event Action<Skill> SkillCreated;
    public event Action<List<Skill>> SkillsInitialized;
    public event Action<List<StatData>> StatsInitialized;
    public event Action<Skill, List<SkillVariant>> SkillsMerged;

    private SkillVariant[] _allSkills;
    private DatabaseReference _skillsReference;
    private DatabaseReference _userReference;

    public string GetAllSkillsPrompt()
    {
        if (_allSkills.Length == 0) return "There is no registered skills in system yet.";
        return _allSkills.Aggregate("Here are all the skills that you have already created: ",
            (current, skill) => current + "[" + skill + "] ") + ". You can use them or come up with new ones";
    }
    
    private void Awake()
    {
        _skillsReference = FirebaseDatabase.DefaultInstance.GetReference("Skills");
        _userReference = FirebaseDatabase.DefaultInstance.GetReference("Users").Child(_user.GetID());
        
        StartCoroutine(GetAllSkills());
        StartCoroutine(GetUserSkills());
        StartCoroutine(GetUserStats());
    }

    private void OnEnable()
    {
        _skillSystem.SkillCreating += OnSkillCreating;
        _user.ExperienceEarned += OnExperienceEarned;
        _user.SkillEvoluted += OnSkillEvolution;
        _skillSystem.SkillsMerging += OnSkillsMerging;

    }
    private void OnDisable()
    {
        _skillSystem.SkillCreating -= OnSkillCreating;
        _user.ExperienceEarned -= OnExperienceEarned;
        _user.SkillEvoluted -= OnSkillEvolution;
        _skillSystem.SkillsMerging -= OnSkillsMerging;
    }
    private void OnSkillCreating(SkillVariant skillVariant)
    {
        StartCoroutine(HandleSkillAdding(skillVariant));
    }

    private void OnExperienceEarned(SkillVariant skillVariant, Level level)
    {
        StartCoroutine(UpdateSkillExperience(skillVariant, level));
        StartCoroutine(UpdateUserStats());
    }

    private IEnumerator UpdateSkillExperience(SkillVariant skillVariant, Level level)
    {
        string jsonData = JsonUtility.ToJson(level);
        yield return _userReference.Child("Skills").Child(skillVariant.Name)
            .Child("CurrentLevel").SetRawJsonValueAsync(jsonData);
    }

    private void OnSkillEvolution(SkillVariant oldVariant, Skill newSkill)
    {
        StartCoroutine(EvoluteSkill(oldVariant, newSkill));
    }

    private IEnumerator EvoluteSkill(SkillVariant oldVariant, Skill newSkill)
    {
        yield return _userReference.Child("Skills").Child(oldVariant.Name).RemoveValueAsync();
        string jsonData = JsonUtility.ToJson(newSkill.GetData());
        yield return _userReference.Child("Skills").Child(newSkill.GetName()).SetRawJsonValueAsync(jsonData);
    }

    private void OnSkillsMerging(SkillVariant newVariant, List<SkillVariant> mergedSkills)
    {
        StartCoroutine(MergeSkills(newVariant, mergedSkills));
    }

    private IEnumerator MergeSkills(SkillVariant newVariant, List<SkillVariant> mergedSkills)
    {
        foreach (SkillVariant mergedSkill in mergedSkills)
        {
            yield return _userReference.Child("Skills").Child(mergedSkill.Name).RemoveValueAsync();
        }
        Skill skill = new Skill(newVariant, _skillsLevelHandler);
        string jsonData = JsonUtility.ToJson(skill.GetData());
        yield return _userReference.Child("Skills").Child(newVariant.Name).SetRawJsonValueAsync(jsonData);
        SkillsMerged?.Invoke(skill, mergedSkills);
    }

    private IEnumerator HandleSkillAdding(SkillVariant skillVariant)
    {
        yield return GetAllSkills();
        bool find = false;
        foreach (SkillVariant variant in _allSkills)
        {
            if (variant.Name == skillVariant.Name)
            {
                find = true;
            }
        }
        
        if (find == false)
        {
            yield return AddSkill(skillVariant);
        }

        yield return CreateUserSkill(skillVariant);
    }
    
    private IEnumerator AddSkill(SkillVariant skillVariant)
    {
        string jsonData = JsonUtility.ToJson(skillVariant);
        yield return _skillsReference.Child(skillVariant.Name).SetRawJsonValueAsync(jsonData);
    }

    private IEnumerator CreateUserSkill(SkillVariant skillVariant)
    {
        Skill skill = new Skill(skillVariant, _skillsLevelHandler);
        string jsonData = JsonUtility.ToJson(skill.GetData());
        yield return _userReference.Child("Skills").Child(skillVariant.Name).SetRawJsonValueAsync(jsonData);
        SkillCreated?.Invoke(skill);
        StartCoroutine(UpdateUserStats());
    }

    private IEnumerator UpdateUserStats()
    {
        StatData[] statsData = _user.GetData().Stats;
        foreach (StatData statData in statsData)
        {
            string jsonData = JsonUtility.ToJson(statData);
            yield return _userReference.Child("Stats").Child(statData.Stat.ToString()).SetRawJsonValueAsync(jsonData);
        }
    }

    private IEnumerator GetAllSkills()
    {
        Task<DataSnapshot> serverData = _skillsReference.GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);

        List<SkillVariant> skillVariants = new List<SkillVariant>();
        foreach (DataSnapshot dataSnapshot in serverData.Result.Children)
        {
            JSONNode json = JSONNode.Parse(dataSnapshot.GetRawJsonValue());
            SkillVariant skillVariant = JsonParser.ParseSkill(json);
            skillVariants.Add(skillVariant);
        }

        _allSkills = skillVariants.ToArray();
    }

    private IEnumerator GetUserSkills()
    {
        Task<DataSnapshot> serverData = _userReference.Child("Skills").GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);

        List<Skill> skillVariants = new List<Skill>();
        foreach (DataSnapshot dataSnapshot in serverData.Result.Children)
        {
            JSONNode json = JSONNode.Parse(dataSnapshot.GetRawJsonValue());
            Skill skillVariant = JsonParser.ParseUserSkill(json, _skillsLevelHandler);
            skillVariants.Add(skillVariant);
        }
        
        SkillsInitialized?.Invoke(skillVariants);
    }
    
    private IEnumerator GetUserStats()
    {
        Task<DataSnapshot> serverData = _userReference.Child("Stats").GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);

        List<StatData> stats = new List<StatData>();
        foreach (DataSnapshot dataSnapshot in serverData.Result.Children)
        {
            StatData stat = JsonUtility.FromJson<StatData>(dataSnapshot.GetRawJsonValue());
            stats.Add(stat);
        }
        
        StatsInitialized?.Invoke(stats);
    }
}
