using UnityEngine;

public class AspectManager : MonoBehaviour
{
    float setWidth;
    float setHeight;

    private void Start()
    {
        Adjust();
    }

    private void Update()
    {
        if (Screen.width != setWidth || Screen.height != setHeight)
        {
            Adjust();
        }
    }

    public void Adjust()
    {
        float targetAspect = 16f / 9f;

        float windowAspect = (float)Screen.width / (float)Screen.height;

        float scaleHeight = windowAspect / targetAspect;

        Camera camera = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }

        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;    
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }

        setHeight = Screen.height;
        setWidth = Screen.width;
    }
}