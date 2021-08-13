using System.Collections;
using System.IO;
using UnityEngine;

public class WebcamCapture : MonoBehaviour
{
    public bool SaveImages;

    private WebCamTexture _webCamTexture;
    private int _photoIndex = 1;

    private void Awake()
    {
        _webCamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = _webCamTexture;
        _webCamTexture.Play();
    }

    private void Update()
    {
        StartCoroutine(TakePhoto());
    }

    IEnumerator TakePhoto()
    {
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(_webCamTexture.width, _webCamTexture.height);
        photo.SetPixels(_webCamTexture.GetPixels());
        photo.Apply();

        byte[] bytes = photo.EncodeToPNG();

        if (SaveImages)
            File.WriteAllBytes("C:\\Temp\\MLVisionSnake\\image_" + (_photoIndex++) + ".png", bytes);

        GameManager.Instance.PredictDirection(bytes);
    }
}
