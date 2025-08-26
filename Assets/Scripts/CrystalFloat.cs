using System;
using Unity.VisualScripting;
using UnityEngine;

public class CrystalFloat : MonoBehaviour
{
    [Serializable]
    public struct CrystalMotionProperties
    {
        public float speed;
        public float xRotate;
        public float yRotate;
        public float zRotate;
    }
    
    [SerializeField] public CrystalMotionProperties properties = new CrystalMotionProperties();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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
    }
}