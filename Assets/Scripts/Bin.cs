using UnityEngine;

public class Bin : MonoBehaviour
{
    private CrystalMovable crystal;


    //Marks the crystal for destruction once it enters the bin area
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.transform.CompareTag("CrystalMovable"))
        {
            crystal = other.transform.GetComponent<CrystalMovable>();
            crystal.inDestructionArea = true;
        }
    }

    //Sets the crystal as not for destruction once it exits the bin area
    private void OnTriggerExit(UnityEngine.Collider other)
    {
        if (other.transform.CompareTag("CrystalMovable"))
        {
            crystal.inDestructionArea = false;
        }
    }
}
