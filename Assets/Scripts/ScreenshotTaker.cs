using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScreenshotTaker : MonoBehaviour
{
    private const string bucket = "galactic-gallery-84c83.firebasestorage.app";

    public Texture2D uploadImage;
    [HideInInspector] public string userName = "Anonymous";

    public void Upload(Texture2D image)
    {
        StartCoroutine(UploadMedia(image));
    }

    public void UploadLastCapture()
    {
        StartCoroutine(UploadMedia(uploadImage));
    }

    private IEnumerator UploadMedia(Texture2D image)
    {
        byte[] imageBytes = image.EncodeToJPG();
        string fileName = $";;{userName};;{DateTime.UtcNow:dd-MM-yyyy};;{DateTime.UtcNow:hhmmss};;.jpg";

        // Use media upload (simplest form)
        string url = $"https://firebasestorage.googleapis.com/v0/b/{bucket}/o?uploadType=media&name={UnityWebRequest.EscapeURL(fileName)}";

        Debug.Log("Uploading to URL: " + url);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(imageBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "image/jpg");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error uploading: " + request.error);
            Debug.Log("Response text: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Upload success");
            Debug.Log("Response: " + request.downloadHandler.text);

            // Optionally parse JSON
            try
            {
                UploadResponse resp = JsonUtility.FromJson<UploadResponse>(request.downloadHandler.text);
                Debug.Log("Name: " + resp.name + " Tokens: " + resp.downloadTokens);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Parse failed: " + ex.Message);
            }
        }
    }

    [Serializable]
    class UploadResponse
    {
        public string name;
        public string bucket;
        public string downloadTokens;
    }
}
