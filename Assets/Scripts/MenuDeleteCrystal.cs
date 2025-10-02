using UnityEngine;

public class MenuDeleteCrystal : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crystal Movable"))
        {
            Destroy(other.gameObject);
        }
    }


}
