using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EdenAI;
using SimpleJSON;

public class SkillSystem : MonoBehaviour
{
    [SerializeField] private User _user;
    [SerializeField] private TaskHandler _taskHandler;
    [SerializeField] private DatabaseBridge _databaseBridge;
    [SerializeField] private LevelHandler _skillsLevelHandler;
    [Multiline][SerializeField] private string _initMessage;

    public event Action<SkillVariant> SkillCreating; 
    public event Action<SkillVariant, int> ExperienceEarned; 
    public event Action<SkillVariant, Skill> SkillEvolution; 
    public event Action<SkillVariant, List<SkillVariant>> SkillsMerging; 

    private static readonly EdenAIApi AI = new EdenAIApi("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiNzc1MzNhNTAtNjE0YS00MzA0LTgyZjEtZjgxZWY0MjdiZDFiIiwidHlwZSI6ImFwaV90b2tlbiJ9.JTiT1WPfwPm2VdP3yKbnNEBxC5HXm5Og69sEsbtzinI");
    private const string Provider = "openai";

    private void OnEnable()
    {
        _taskHandler.TaskEnded += OnTaskEnd;
    }

    private void OnDisable()
    {
        _taskHandler.TaskEnded -= OnTaskEnd;
    }

    private async void OnTaskEnd(TimedTask task)
    {
        List<ChatMessage> settings = new List<ChatMessage>() {
            new ChatMessage()
            {
                Message = _initMessage,
                Role = "assistant"
            },
            new ChatMessage()
            {
                Message = _databaseBridge.GetAllSkillsPrompt(),
                Role = "assistant"
            }
        };
        
        string request = $"I spent the last {task.GetTime()}(mm:ss) by doing task {task.GetTitle()}({task.GetDescription()}), {_user.GetSkillPrompt()} Based on this information manage suitable skills and stats.";
        ChatResponse response = await AI.SendChatRequest(Provider, request, previousHistory: settings);
        ChatMessage answer = response.message.Last();
        
        Debug.Log(answer.Message);
        
        JSONNode jsonAnswer = JSONNode.Parse(answer.Message);
        foreach (JSONNode action in jsonAnswer["Actions"].Children)
            HandleJson(action);
    }

    private void HandleJson(JSONNode actionJson)
    {
        ActionType actionType = (ActionType) actionJson["ActionType"].AsInt;
        SkillVariant skillVariant = JsonParser.ParseSkill(actionJson["Skill"]);
        switch (actionType)
        {
            case ActionType.CreatingSkill:
                SkillCreating?.Invoke(skillVariant);
                break;
            case ActionType.AddingExperience:
                int experience = actionJson["Experience"].AsInt;
                ExperienceEarned?.Invoke(skillVariant, experience);
                break;
            case ActionType.SkillEvolution:
                Skill newVariant = JsonParser.ParseUserSkill(actionJson["OldSkill"], _skillsLevelHandler);
                SkillEvolution?.Invoke(skillVariant, newVariant);
                break;
            case ActionType.SkillsMerging:
                List<SkillVariant> mergedSkills = new List<SkillVariant>();
                foreach (JSONNode mergedSkill in actionJson["MergedSkills"].Children)
                    mergedSkills.Add(JsonParser.ParseSkill(mergedSkill));
                SkillsMerging?.Invoke(skillVariant, mergedSkills);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    
}
