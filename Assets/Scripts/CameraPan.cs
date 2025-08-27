using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public AnimationCurve streamPanDamping;
    public AnimationCurve canvasPanDamping;
    float time;
    public float panTime;

    bool panningToStream = false;
    bool panningToCanvas = false;

    float startY;

    void Update()
    {
        if (panningToStream)
        {
            startY = transform.position.y;

            transform.rotation = Quaternion.Euler(transform.rotation.x, streamPanDamping.Evaluate(time), transform.rotation.z);
            time += Time.deltaTime;

            if (time > panTime) { panningToStream = false; }
            //Deactivate button object, activate other button
        }

        else if (panningToCanvas)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, canvasPanDamping.Evaluate(time), transform.rotation.z);
            time += Time.deltaTime;

            if (time > panTime) { panningToCanvas = false; }
            //Deactivate button object, activate other button
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