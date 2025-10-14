using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static System.Net.Mime.MediaTypeNames;
using Unity.VisualScripting;

public class JuicedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Values")]
    public Button thisButton;
    float aspectRatio;

    [Header("Grow Variables")]
    public AnimationCurve growCurve;
    public float growTime;
    float growTimer = 0f;

    [Header("Tutorialisation values")]
    [HideInInspector] public bool buttonClicked = false;
    public float firstClickTimer;
    public GameObject tutorialisationEffect;
    GameObject newEffect;
    bool tutorialising;
    Vector3 baseScale;

    private void Start()
    {
        aspectRatio = thisButton.transform.localScale.x / thisButton.transform.localScale.y;
        baseScale = thisButton.transform.localScale;
    }

    private void Update()
    {
        //If the button hasn't been clicked within the amount of seconds allowed by first click timer, instantiate a particle effect and start pulsating
        if (!buttonClicked && Time.time > firstClickTimer && !tutorialising)
        {
            newEffect = Instantiate(tutorialisationEffect);
            newEffect.transform.position = transform.position;
            tutorialising = true;
            //StartCoroutine(ButtonPulsate());
        }
        if (buttonClicked)
        {
            if (transform.localScale.x > baseScale.x) { StartCoroutine(ButtonShrink()); }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Over button");
        StartCoroutine(ButtonGrow());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse left Button");
        StartCoroutine(ButtonShrink());
    }

    private IEnumerator ButtonGrow()
    {
        while (growTimer < growTime)
        {
            thisButton.transform.localScale = new Vector3(baseScale.x + growCurve.Evaluate(growTimer), baseScale.y + growCurve.Evaluate(growTimer), baseScale.z);
            growTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator ButtonShrink()
    {
        Vector3 startScale = transform.localScale;
        while (growTimer > 0)
        {
            thisButton.transform.localScale = new Vector3(startScale.x - growCurve.Evaluate(growTimer), startScale.y - growCurve.Evaluate(growTimer), baseScale.z);
            growTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator ButtonPulsate()
    {
        while (!buttonClicked)
        {
            while (growTimer < growTime)
            {
                thisButton.transform.localScale = new Vector3(this.transform.localScale.x + (growCurve.Evaluate(growTimer) * aspectRatio), this.transform.localScale.y + growCurve.Evaluate(growTimer), 1f);
                growTimer += Time.deltaTime;
                newEffect.transform.localScale = thisButton.transform.localScale;
            }
            while (growTimer > 0)
            {
                thisButton.transform.localScale = new Vector3(this.transform.localScale.x - (growCurve.Evaluate(growTimer) * aspectRatio), this.transform.localScale.y - growCurve.Evaluate(growTimer), 1f);
                growTimer -= Time.deltaTime;
                newEffect.transform.localScale = thisButton.transform.localScale;
            }
        }
        yield return null;
    }
}