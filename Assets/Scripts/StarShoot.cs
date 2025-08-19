using UnityEngine;

public class StarShoot : MonoBehaviour
{
    public float speed = 10.0f;

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.forward * speed;
    }
}