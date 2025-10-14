
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenTransition : MonoBehaviour
{
    public Image image; 

    private void Awake()
    {
        if (image != null)
        {
            Color c = image.color;
            c.a = 0f;
            image.color = c;
        }
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
            image.color = c;
            yield return null;
        }

    }
    public IEnumerator FadeOut(float duration)
    {
        float t = 0f;
        Color c = image.color;

        while (t < 1)
        {
            t += Time.deltaTime;
            c.a = 1f - (t / 1f);
            image.color = c;
            yield return null;
        }
    }
}
    