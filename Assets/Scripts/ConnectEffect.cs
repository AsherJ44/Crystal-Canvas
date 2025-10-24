using UnityEngine;

public class ConnectEffect : MonoBehaviour
{
    [SerializeField] private float baseScaleRange;
    [SerializeField] private float baseScaleFactor;

    float minScale;
    float maxScale;
    float baseScale;
    float scaleRange;
    float scaleFactor;

    [SerializeField] private float baseRotationRange;
    [SerializeField] private float baseRotationFactor;

    float minRotation;
    float maxRotation;
    float rotationRange;
    float rotationFactor;

    bool growing = true;
    bool tiltingUp = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseScale = this.transform.localScale.y;
        scaleRange = Random.Range(0, baseScaleRange);

        minScale = baseScale - scaleRange;
        maxScale = baseScale + scaleRange;

        scaleFactor = Random.Range(0.001f, baseScaleFactor);

        rotationFactor = Random.Range(0.1f, baseRotationFactor);
        rotationRange = Random.Range(0f, baseRotationRange);

        minRotation = 0 - rotationRange;
        maxRotation = rotationRange;

        if (Random.Range(0, 2) == 0 ) { growing = false; }
        if (Random.Range(0, 2) == 0 ) { tiltingUp = false; }
    }

    // Update is called once per frame
    void Update()
    {
        if (growing)
        {
            if (transform.localScale.y <= maxScale)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + scaleFactor * Time.deltaTime, transform.localScale.z);
            }
            else { growing = false; }
        }
        else if (!growing)
        {
            if (transform.localScale.y >= minScale)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - scaleFactor * Time.deltaTime, transform.localScale.z);
            }
            else { growing = true; }
        }
        /*
        if (tiltingUp)
        {
            if (transform.eulerAngles.z <= maxRotation)
            {
                transform.Rotate(0, 0, rotationFactor * Time.deltaTime);
            }
            else { tiltingUp = false; }
        }
        else if (!tiltingUp)
        {
            if (transform.eulerAngles.z >= minRotation)
            {
                transform.Rotate(0, 0, -(rotationFactor * Time.deltaTime));
            }
            else { tiltingUp = true; }
        }
        */
    }
}