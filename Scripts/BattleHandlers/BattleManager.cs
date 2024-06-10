using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Storage;
using SimpleJSON;
using UnityEngine.Networking;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private QrCodeScanner _qrCodeScanner;
    [SerializeField] private RawImage _opponentIcon;
    [SerializeField] private LevelHandler _skillsLevelHandler;
    [SerializeField] private BattleView _battleView;

    public event Action<List<Skill>> OpponentSkillsGot;
    public event Action<List<StatData>> OpponentStatsGot;
    
    private DatabaseReference _opponentReference;
    private StorageReference _userIconReference;

    private string _userID;
    private string _opponentID;

    private void Start()
    {
        _userID = SystemInfo.deviceUniqueIdentifier;
    }

    private void OnEnable()
    {
        _qrCodeScanner.QrScanned += OnOpponentFound;
        _battleView.BattleEnd += OnBattleEnd;
    }

    private void OnDisable()
    {
        _qrCodeScanner.QrScanned -= OnOpponentFound;
        _battleView.BattleEnd -= OnBattleEnd;
    }

    private void OnOpponentFound(string opponentID)
    {
        StartCoroutine(HandleOpponent(opponentID));
    }

    private IEnumerator HandleOpponent(string opponentID)
    {
        _opponentID = opponentID;
        
        StorageReference storageReference = FirebaseStorage.DefaultInstance.GetReference("Icons");
        _userIconReference = storageReference.Child(opponentID);
        Task<Uri> serverData = _userIconReference.GetDownloadUrlAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);
        StartCoroutine(LoadImage(Convert.ToString(serverData.Result)));
        
        _opponentReference = FirebaseDatabase.DefaultInstance.GetReference("Users").Child(opponentID);
        
        Task<DataSnapshot> snapshot = _opponentReference.Child("Skills").GetValueAsync();
        yield return new WaitUntil(() => snapshot.IsCompleted);
        List<Skill> skills = new List<Skill>();
        foreach (DataSnapshot dataSnapshot in snapshot.Result.Children)
        {
            JSONNode json = JSONNode.Parse(dataSnapshot.GetRawJsonValue());
            Skill skillVariant = JsonParser.ParseUserSkill(json, _skillsLevelHandler);
            skills.Add(skillVariant);
        }
        OpponentSkillsGot?.Invoke(skills);
        
        snapshot = _opponentReference.Child("Stats").GetValueAsync();
        yield return new WaitUntil(() => snapshot.IsCompleted);
        List<StatData> stats = new List<StatData>();
        foreach (DataSnapshot dataSnapshot in snapshot.Result.Children)
        {
            StatData stat = JsonUtility.FromJson<StatData>(dataSnapshot.GetRawJsonValue());
            stats.Add(stat);
        }
        OpponentStatsGot?.Invoke(stats);
    }

    private void OnBattleEnd(bool isWin)
    {
        StartCoroutine(isWin ? SaveBattle(_userID, _opponentID) : SaveBattle(_opponentID, _userID));
    }
    
    private IEnumerator SaveBattle(string winnerID, string loserID)
    {
        DatabaseReference battleReference = FirebaseDatabase.DefaultInstance.GetReference("Battles").Child(DateTime.Now.ToString("YYYY-MM-DD HH:mm:ss"));
        yield return battleReference.Child("Winner").SetValueAsync(winnerID);
        yield return battleReference.Child("Loser").SetValueAsync(loserID);
    }
    
    private IEnumerator LoadImage(string url){
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url); 
        yield return request.SendWebRequest();
        if(request.isNetworkError || request. isHttpError){
            // pass
        }
        else{
            _opponentIcon.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            yield return _userIconReference.PutFileAsync(url);
        }
    }
}
