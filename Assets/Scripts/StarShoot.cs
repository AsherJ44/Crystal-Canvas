using UnityEngine;

public class StarShoot : MonoBehaviour
{
    [HideInInspector] public float speed;

    private void Start()
    {
        Destroy(this.gameObject, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.forward * speed;
    }
}