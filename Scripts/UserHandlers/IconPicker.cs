using System;
using System.Collections;
using Kakera;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase.Storage;

public class IconPicker : MonoBehaviour
{
    [SerializeField] private Unimgpicker _imagePicker;
    [SerializeField] private RawImage _imageRenderer;
    [SerializeField] private Button _setButton;
    
    private StorageReference _storageReference;
    private StorageReference _userIconReference;
    private void OnEnable()
    {
        _imagePicker.Completed += OnPick;
        _setButton.onClick.AddListener(OnPressShowPicker);
    }

    private void OnDisable()
    {
        _imagePicker.Completed -= OnPick;
        _setButton.onClick.RemoveListener(OnPressShowPicker);
    }
    
    private void Start()
    {
        _storageReference = FirebaseStorage.DefaultInstance.GetReference("Icons");
        _userIconReference = _storageReference.Child(SystemInfo.deviceUniqueIdentifier);

        _userIconReference.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if(!task.IsFaulted && !task.IsCanceled){
                StartCoroutine(LoadImage(Convert.ToString(task.Result)));
            }
            else{
                Debug.Log(task.Exception);
            }
        });
    }

    private void OnPressShowPicker()
    {
        _imagePicker.Show("Select Image", "icon");
    }

    private void OnPick(string path)
    {
        StartCoroutine(LoadImage("file://" + path));
    }
    
    private IEnumerator LoadImage(string url){
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url); 
        yield return request.SendWebRequest();
        if(request.isNetworkError || request. isHttpError){
            Debug.Log(request.error);
        }
        else{
            _imageRenderer.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            yield return _userIconReference.PutFileAsync(url);
        }
    }
}
