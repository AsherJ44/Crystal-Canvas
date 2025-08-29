using System.IO;
using UnityEngine;

public class CameraOutputSaver : MonoBehaviour
{
    public Camera targetCamera; // Assign the camera in the Inspector
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public string folderName = "CameraCaptures";
    private int captureIndex = 0;

    public GameObject submitButton;

    void Start()
    {
        if (targetCamera == null)
        {
            Debug.LogError("Target Camera not assigned!");
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) // Press 'C' to capture
        {
            CaptureAndSaveCameraOutput();
        }
    }

    public void CaptureAndSaveCameraOutput()
    {
        RenderTexture rt = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        targetCamera.targetTexture = rt;
        Texture2D screenshot = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        targetCamera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenshot.EncodeToPNG(); // or EncodeToJPG
        string path = Path.Combine(Application.dataPath, folderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string fileName = string.Format("{0}/capture_{1:000}.png", path, captureIndex);
        File.WriteAllBytes(fileName, bytes);
        Debug.Log("Camera output saved to: " + fileName);
        captureIndex++;

        submitButton.SetActive(true);

        Destroy(screenshot);
    }
}