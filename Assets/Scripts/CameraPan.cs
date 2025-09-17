using UnityEngine;

public class CameraPan : MonoBehaviour
{
    [SerializeField] private AnimationCurve streamPanDamping;
    [SerializeField] private AnimationCurve canvasPanDamping;
    
    [SerializeField] private float panTime;

    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;

    [SerializeField] private GameObject lightsButton;
    [SerializeField] private GameObject captureButton;

    [HideInInspector] public GameManager manager;

    [SerializeField] private GameObject Bin;

    private bool onCanvas = true;
    private bool panningToStream = false;
    private bool panningToCanvas = false;

    private float startY;
    private float time;

    public int timesPanned;

    void Start()
    {
        leftButton.SetActive(true);
        rightButton.SetActive(false);
        manager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (panningToStream)
        {
            onCanvas = false;

            startY = transform.position.y;

            transform.rotation = Quaternion.Euler(transform.rotation.x, streamPanDamping.Evaluate(time), transform.rotation.z);
            time += Time.deltaTime;

            if (manager.canvasCrystals.Count > 0) { manager.DeactivateCrystals(); }
                
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
            onCanvas = true;

            if (time > panTime) { panningToCanvas = false; }
        }

        //Only setting the light button to active if the player has placed crystals
        if (manager.canvasCrystals.Count > 0 && onCanvas) { lightsButton.SetActive(true); }
        else { lightsButton.SetActive(false); }
    }

    public void PanToStream()
    {
        time = 0;
        timesPanned++;
        panningToStream = true;
    }

    public void PanToCanvas()
    {
        time = 0;
        panningToCanvas = true;
    }
}