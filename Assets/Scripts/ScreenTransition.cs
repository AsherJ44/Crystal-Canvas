using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private MenuHandler menuHandler;
    [SerializeField] private int sceneToLoad;
    [SerializeField] private float fadeInTime;
    [SerializeField] private AudioSource backgroundMusic;
    float baseVolume;

    private void Awake()
    {
        if (image != null)
        {
            Color c = image.color;
            c.a = 1f;
            image.color = c;
        }

        if (backgroundMusic != null) { baseVolume = backgroundMusic.volume; }
        StartCoroutine(FadeFromBlack(fadeInTime));
    }

    public void FadeAndLoad(float duration)
    {
        StartCoroutine(FadeToBlackAndLoad(duration));
    }

    public IEnumerator FadeToBlackAndLoad(float duration)
    {
        float t = 0f;
        Color c = image.color;

        // Fade to black
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = t / duration;
            backgroundMusic.volume = baseVolume - ((t / duration) * baseVolume);
            image.color = c;
            yield return null;
        }
        menuHandler.LoadScene(sceneToLoad);
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        float t = 0f;
        Color c = image.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = 1f - (t / duration);
            backgroundMusic.volume = (t / duration) * baseVolume;
            image.color = c;
            yield return null;
        }
    }
}
    