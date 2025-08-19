using UnityEngine;

public class StarShoot : MonoBehaviour
{
    [HideInInspector] public float speed;

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.forward * speed;
    }
}