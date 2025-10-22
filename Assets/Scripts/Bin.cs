using UnityEngine;
using System.Collections;

public class Bin : MonoBehaviour
{
    private CrystalMovable crystal;
    [HideInInspector] public bool crystalDropped;
    [HideInInspector] public bool crystalHeld;

    [Header("Grow Variables")]
    public AnimationCurve growCurve;
    public AnimationCurve shrinkCurve;
    public float growTime;
    float growTimer = 0f;

    Vector3 baseScale;

    private void Start()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        //if (crystalDropped) { StopAllCoroutines(); }
    }

    public void CrystalPickedUp()
    {
        if (!crystalHeld)
        {
            crystalHeld = true;
            growTimer = 0f;
            StartCoroutine(BinPulsate());
        }
    }

    public void CrystalDropped()
    {
        crystalHeld = false;
        StopAllCoroutines();
        crystalDropped = true;
        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        while (growTimer < growTime && transform.localScale.x > baseScale.x)
        {
            transform.localScale = new Vector3(baseScale.x + shrinkCurve.Evaluate(growTimer), baseScale.y + shrinkCurve.Evaluate(growTimer), baseScale.z);
            growTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator BinPulsate()
    {
        while (crystalHeld)
        {
            while (growTimer < growTime)
            {
                transform.localScale = new Vector3(baseScale.x + growCurve.Evaluate(growTimer), baseScale.y + growCurve.Evaluate(growTimer), baseScale.z);
                growTimer += Time.deltaTime / 2; //Dividing delta time by 5 makes the animation slower without requiring new curves
                yield return new WaitForEndOfFrame();
            }

            growTimer = 0;

            while (growTimer < growTime)
            {
                transform.localScale = new Vector3(baseScale.x + shrinkCurve.Evaluate(growTimer), baseScale.y + shrinkCurve.Evaluate(growTimer), baseScale.z);
                growTimer += Time.deltaTime / 2;
                yield return new WaitForEndOfFrame();
            }
            growTimer = 0;
        }
        yield return null;
    }
}
