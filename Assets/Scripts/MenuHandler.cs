using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public void LoadScene(int sceneInt)
    {
        SceneManager.LoadScene(sceneInt);
    }
}