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
        Debug.Log($"x rotate is {this.transform.rotation.x}; change is {properties.xRotate}");
        transform.position = new Vector3(transform.position.x, this.transform.position.y - (properties.speed * Time.deltaTime), transform.position.z);
        transform.rotation = new Quaternion(this.transform.rotation.x + (properties.xRotate),
                                            this.transform.rotation.y + (properties.yRotate * Time.deltaTime),
                                            this.transform.rotation.z + (properties.zRotate * Time.deltaTime), 0);
    }
}