using UnityEngine;

public class ShareFrame : MonoBehaviour
{
    [HideInInspector] public bool slidingUp;
    [HideInInspector] public bool slidingDown;

    public AnimationCurve slideUpCurve;
    public AnimationCurve slideDownCurve;

    float slideTimer;
    public float slideTime;
    Vector3 newPosition;
    RectTransform rectTransform;

    void OnEnable()
    {
        slidingUp = true;
        slideTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (slidingUp && slideTimer < slideTime)
        {
            newPosition = new Vector3(0, slideUpCurve.Evaluate(slideTimer), 0);
            rectTransform = (RectTransform)transform;
            rectTransform.anchoredPosition = newPosition;
            slideTimer += Time.deltaTime;
        }
        else if (slidingUp && slideTimer >= slideTime)
        {
            slidingUp = false;
        }

        if (slidingDown && slideTimer < slideTime)
        {
            newPosition = new Vector3(0, slideDownCurve.Evaluate(slideTimer), 0);
            rectTransform = (RectTransform)transform;
            rectTransform.anchoredPosition = newPosition;
            slideTimer += Time.deltaTime;
        }
        else if (slidingDown && slideTimer >= slideTime)
        {
            slidingDown = false;
            this.gameObject.SetActive(false);
        }
    }

    public void Deactivate()
    {
        slidingDown = true;
        slideTimer = 0;
    }
}
