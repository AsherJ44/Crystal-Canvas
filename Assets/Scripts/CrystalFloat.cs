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


    public struct CrystalMotionProperties
    {
        public float speed;
        public float xRotate;
        public float yRotate;
        public float zRotate;
    }
    
    public CrystalMotionProperties properties = new CrystalMotionProperties();

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, this.transform.position.y - (properties.speed * Time.deltaTime), transform.position.z);
        this.transform.Rotate(properties.xRotate * Time.deltaTime, properties.yRotate * Time.deltaTime,
                         properties.zRotate * Time.deltaTime, Space.Self);
        if (transform.position.y < -0.3f)
        {
            Destroy(gameObject);
        }

        if (clickedAndMoving)
        {
            transform.position = Vector3.Lerp(startPos, canvasPos, lerpLevel);
            lerpLevel += 0.01f;
        }
    }

    private void OnMouseDown()
    {
        //Store a reference of the crystal's current position
        startPos = transform.position;
        
        //Set random position within the canvas bounds
        canvasPos = new Vector3(-0.25f, UnityEngine.Random.Range(-0.087f, 0.105f), UnityEngine.Random.Range(-0.195f, 0.195f));

        //Setting the crystal to start lerping over to the canvas
        clickedAndMoving = true;

        //Set crystal movable script to active
        CrystalMovable crystalMovable = GetComponent<CrystalMovable>();
        crystalMovable.enabled = false;
    }
}