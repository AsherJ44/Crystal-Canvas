using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CrystalFloat : MonoBehaviour
{
    bool clickedAndMoving = false;
    float lerpLevel = 0.0f;
    Vector3 startPos = new Vector3();
    Vector3 canvasPos = new Vector3();

    bool waiting = false;

    [HideInInspector] public GameManager manager;

    public struct CrystalMotionProperties
    {
        public float speed;
        public float xRotate;
        public float yRotate;
        public float zRotate;
    }
    
    public CrystalMotionProperties properties = new CrystalMotionProperties();

    private void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, this.transform.position.y - (properties.speed * Time.deltaTime), transform.position.z);
        this.transform.Rotate(properties.xRotate * Time.deltaTime, properties.yRotate * Time.deltaTime,
                         properties.zRotate * Time.deltaTime, Space.Self);

        //Destroys the crystal once it goes low enough
        if (transform.position.y < -0.45f)
        {
            Destroy(gameObject);
        }

        //Moves the crystal over to the canvas and waits for 2 seconds before activating the crystal movable component
        if (clickedAndMoving)
        {
            transform.position = Vector3.Lerp(startPos, canvasPos, lerpLevel);
            lerpLevel += Time.deltaTime;

            if (!waiting)
            {
                waiting = true;
                StartCoroutine(WaitToActivate());
            }
        }
    }

    private IEnumerator WaitToActivate()
    {
        yield return new WaitForSeconds(2.0f);
        CrystalMovable crystalMovable = GetComponent<CrystalMovable>();
        crystalMovable.enabled = true;
        manager.canvasCrystals.Add(crystalMovable);
        crystalMovable.onCanvas = true;
    }

    private void OnMouseDown()
    {
        //Store a reference of the crystal's current position
        startPos = transform.position;
        
        //Set random position within the canvas bounds
        canvasPos = new Vector3(-0.25f, UnityEngine.Random.Range(-0.087f, 0.105f), UnityEngine.Random.Range(-0.195f, 0.195f));

        //Setting the crystal to start lerping over to the canvas
        clickedAndMoving = true;
    }
}