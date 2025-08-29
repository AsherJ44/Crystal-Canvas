using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<CrystalMovable> canvasCrystals; //List of all crystals on the canvas, used to turn them on and off
    public GameObject snapShotButton;
    public GameObject uploadButton;

    public void Start()
    {
        snapShotButton.SetActive(false);
        uploadButton.SetActive(false);
    }

    public void ActivateCrystals()
    {
        foreach (CrystalMovable crystal in canvasCrystals)
        {
            crystal.GetComponent<ParticleSystem>();
        }

        snapShotButton.SetActive(true);
    }

    public void OpenGallery(string url)
    {
        Application.OpenURL(url);
    }
}