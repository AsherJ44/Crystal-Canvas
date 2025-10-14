using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.GraphicsBuffer;

public class JuicedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Values")]
    public Button thisButton;
    float aspectRatio;

    [Header("Grow Variables")]
    public AnimationCurve growCurve;
    public AnimationCurve shrinkCurve;
    public float growTime;
    float growTimer = 0f;

    [Header("Tutorialisation values")]
    [HideInInspector] public bool buttonClicked = false;
    public float firstClickTimer;
    public GameObject tutorialisationEffect;
    GameObject newEffect;
    bool tutorialising;
    Vector3 baseScale;
    Vector3 effectBaseScale;
    Camera cam;

    private void Start()
    {
        aspectRatio = thisButton.transform.localScale.x / thisButton.transform.localScale.y;
        baseScale = thisButton.transform.localScale;
        cam = Camera.main;
    }

    private void Update()
    {
        //If the button hasn't been clicked within the amount of seconds allowed by first click timer, instantiate a particle effect and start pulsating
        if (!buttonClicked && Time.time > firstClickTimer && !tutorialising)
        {
            //newEffect = Instantiate(tutorialisationEffect, this.transform);
            //effectBaseScale = newEffect.transform.localScale;
            tutorialising = true;
            StartCoroutine(ButtonPulsate());
        }
        if (buttonClicked)
        {
            if (transform.localScale.x > baseScale.x) { StartCoroutine(ButtonShrink()); }
        }
    }

    public void Clicked()
    {
        buttonClicked = true;
        StopAllCoroutines();
        Destroy(newEffect);
        growTimer = 0;
        StartCoroutine(ButtonShrink());
    }

    public void SizeReset()
    {
        thisButton.transform.localScale = baseScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        if (newEffect != null) { Destroy(newEffect); }
        if (growTimer >= growTime) { growTimer = 0; } //If the button grow effect has completed, set the timer to zero, otherwise let it continue from where it was
        StartCoroutine(ButtonGrow());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        if (growTimer >= growTime) { growTimer = 0; } //If the button grow effect has completed, set the timer to zero, otherwise let it continue from where it was
        StartCoroutine(ButtonShrink());
    }

    private IEnumerator ButtonGrow()
    {
        while (growTimer < growTime && thisButton.transform.localScale.x < baseScale.x + growCurve.Evaluate(growTime))
        {
            thisButton.transform.localScale = new Vector3(baseScale.x + growCurve.Evaluate(growTimer), baseScale.y + growCurve.Evaluate(growTimer), baseScale.z);
            growTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator ButtonShrink()
    {
        while (growTimer < growTime && thisButton.transform.localScale.x > baseScale.x)
        {
            thisButton.transform.localScale = new Vector3(baseScale.x + shrinkCurve.Evaluate(growTimer), baseScale.y + shrinkCurve.Evaluate(growTimer), baseScale.z);
            growTimer += Time.deltaTime;
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
                thisButton.transform.localScale = new Vector3(baseScale.x + growCurve.Evaluate(growTimer), baseScale.y + growCurve.Evaluate(growTimer), baseScale.z);
                growTimer += Time.deltaTime / 5; //Dividing delta time by 5 makes the animation slower without requiring new curves
                yield return new WaitForEndOfFrame();
            }

            growTimer = 0;

            while (growTimer < growTime)
            {
                thisButton.transform.localScale = new Vector3(baseScale.x + shrinkCurve.Evaluate(growTimer), baseScale.y + shrinkCurve.Evaluate(growTimer), baseScale.z);
                growTimer += Time.deltaTime / 5;
                yield return new WaitForEndOfFrame();
            }
            growTimer = 0;
        }
        yield return null;
    }
}