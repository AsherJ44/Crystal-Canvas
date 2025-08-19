using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}