using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QrCodeScanner : MonoBehaviour
{
    [SerializeField] private RawImage _background;
    [SerializeField] private RectTransform _scanZone;
    [SerializeField] private Button _scanButton;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;

    public event Action<string> QrScanned; 
    
    private bool _isCameraAvailable;
    private WebCamTexture _webCamTexture;

    private void Start()
    {
        SetUpCamera();
    }

    private void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (!_isCameraAvailable) return;
        _aspectRatioFitter.aspectRatio = (float)_webCamTexture.width / _webCamTexture.height;
        int zRotation = - _webCamTexture.videoRotationAngle;
        _background.rectTransform.localEulerAngles = new Vector3(0, 0, zRotation);
    }

    private void OnEnable()
    {
        _scanButton.onClick.AddListener(Scan);
    }

    private void OnDisable()
    {
        _scanButton.onClick.RemoveListener(Scan);
    }

    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            _isCameraAvailable = false;
            return;
        }

        foreach (WebCamDevice device in devices)
        {
            if (device.isFrontFacing == false)
            {
                _webCamTexture = new WebCamTexture(device.name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
            }
        }
        _webCamTexture.Play();
        _background.texture = _webCamTexture;
        _isCameraAvailable = true;
    }
    
    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_webCamTexture.GetPixels32(), _webCamTexture.width, _webCamTexture.height);
            if (result != null)
            {
                QrScanned?.Invoke(result.Text);
            }
        }
        catch
        {
            // ignored
        }
    }
}
