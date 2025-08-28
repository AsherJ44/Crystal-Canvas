using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<CrystalMovable> canvasCrystals; //List of all crystals on the canvas, used to turn them on and off

    public void ActivateCrystals()
    {
        foreach (CrystalMovable crystal in canvasCrystals)
        {
            crystal.GetComponent<ParticleSystem>();
        }
    }
}