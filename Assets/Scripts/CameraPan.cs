using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public AnimationCurve streamPanDamping;
    public AnimationCurve canvasPanDamping;
    float time;
    public float panTime;

    public GameObject leftButton;
    public GameObject rightButton;

    public GameObject lightsButton;
    public GameObject captureButton;

    bool panningToStream = false;
    bool panningToCanvas = false;

    float startY;

    void Start()
    {
        leftButton.SetActive(true);
        rightButton.SetActive(false);
    }

    void Update()
    {
        if (panningToStream)
        {
            startY = transform.position.y;

            transform.rotation = Quaternion.Euler(transform.rotation.x, streamPanDamping.Evaluate(time), transform.rotation.z);
            time += Time.deltaTime;

            leftButton.SetActive(false);
            rightButton.SetActive(true);
            captureButton.SetActive(false);
            lightsButton.SetActive(false);

            if (time > panTime) { panningToStream = false; }
        }

        else if (panningToCanvas)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, canvasPanDamping.Evaluate(time), transform.rotation.z);
            time += Time.deltaTime;

            leftButton.SetActive(true);
            rightButton.SetActive(false);
            captureButton.SetActive(false);
            lightsButton.SetActive(true);

            if (time > panTime) { panningToCanvas = false; }
        }
    }

    public void PanToStream()
    {
        time = 0;
        panningToStream = true;
    }

    public void PanToCanvas()
    {
        time = 0;
        panningToCanvas = true;
    }
}