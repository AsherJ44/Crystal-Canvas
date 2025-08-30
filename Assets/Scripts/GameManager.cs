using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using static CrystalSpawn;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<CrystalMovable> canvasCrystals; //List of all crystals on the canvas, used to turn them on and off
    public GameObject snapShotButton;
    public GameObject uploadButton;
    public GameObject crystalActiveButton;
    public GameObject crystalInactiveButton;

    public Color[] crystalColours; //List of potential crystal colours

    public void Start()
    {
        crystalInactiveButton.SetActive(false);
        snapShotButton.SetActive(false);
        uploadButton.SetActive(false);
    }

    public void ActivateCrystals()
    {
        foreach (CrystalMovable crystal in canvasCrystals)
        {
            Color crystalColour = crystalColours[crystal.colourIndex];

            ParticleSystem particles = crystal.crystalEffect.GetComponent<ParticleSystem>();

            crystal.crystalEffect.SetActive(true);
            crystal.crystalEffect.transform.rotation = Quaternion.Euler(0, 0, 80);

            //Setting the colour of the crystal's particle effect
            var pfxMain = particles.main;
            pfxMain.startColor = crystalColour;
            particles.Play();

            //Setting the colour of the crystals point light
            crystal.crystalLight.SetActive(true);
            crystal.crystalLight.GetComponent<Light>().color = crystalColour;
        }

        //Setting relevant buttons active and inactive
        crystalActiveButton.SetActive(false);
        crystalInactiveButton.SetActive(true);
        snapShotButton.SetActive(true);
    }

    public void DeactivateCrystals()
    {
        //Disabling crystal effects
        foreach (CrystalMovable crystal in canvasCrystals)
        {
            //Turning the crystal light and particle system off
            crystal.crystalLight.SetActive(false);
            crystal.crystalEffect.SetActive(true);
        }

        //Setting relevant buttons active and inactive
        crystalActiveButton.SetActive(true);
        crystalInactiveButton.SetActive(false);
        snapShotButton.SetActive(false);
    }

    public void OpenGallery(string url)
    {
        Application.OpenURL(url);
    }
}