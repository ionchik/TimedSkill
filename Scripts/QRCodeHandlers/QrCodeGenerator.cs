using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class QrCodeGenerator : MonoBehaviour
{
    [SerializeField] private RawImage _qrCodeImage;

    private Texture2D _encodedTexture;

    private void Start()
    {
        _encodedTexture = new Texture2D(256, 256);
        Generate(SystemInfo.deviceUniqueIdentifier);
    }

    private Color32[] Encode(string encodingText, int width, int height)
    {
        BarcodeWriter barcodeWriter = new BarcodeWriter()
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions()
            {
                Height = height,
                Width = width
            }
        };
        return barcodeWriter.Write(encodingText);
    }

    private void Generate(string text)
    {
        Color32[] qrTexture = Encode(text, _encodedTexture.width, _encodedTexture.height);
        _encodedTexture.SetPixels32(qrTexture);
        _encodedTexture.Apply();

        _qrCodeImage.texture = _encodedTexture;
    }
}
