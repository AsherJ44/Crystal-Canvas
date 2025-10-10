using UnityEngine;

public class GlobalInstanceManager : MonoBehaviour
{
    public static GlobalInstanceManager Instance;

    public int CameraCaptureIndex;
    public int testerIndex;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes

            // Bump up the framerate
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}