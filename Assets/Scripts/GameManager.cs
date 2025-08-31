using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using static CrystalSpawn;
using System;
using UnityEngine.Experimental.GlobalIllumination;
using static GameManager;
using static UnityEngine.ParticleSystem;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public List<CrystalMovable> canvasCrystals; //List of all crystals on the canvas, used to turn them on and off
    public GameObject snapShotButton;
    public GameObject uploadButton;
    public GameObject crystalActiveButton;
    public GameObject crystalInactiveButton;
    public GameObject cameraPanButton;

    public Light crystalLight;

    [Serializable]
    public struct CrystalColours
    {
        public Color lightColor;
        public Color particleColor;
        public float intensity;
        public Material newMaterial;
        public Material oldMaterial;
    }

    public List<CrystalColours> crystalColours = new List<CrystalColours>();

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
            int crystalIndex = crystal.colourIndex; //Getting the colour reference for the crystal

            Light newCrystalLight = Instantiate(crystalLight, new Vector3(crystal.transform.position.x + 0.02f, crystal.transform.position.y, crystal.transform.position.z), Quaternion.Euler(new Vector3(0,0,0)), crystal.transform);

            //Setting the colour and intensity of the crystals point light
            newCrystalLight.color = crystalColours[crystalIndex].lightColor;
            newCrystalLight.intensity = crystalColours[crystalIndex].intensity;

            //Changing the crystal's material to one with a much higher specular roughness so it glows better
            crystal.GetComponent<MeshRenderer>().material = crystalColours[crystalIndex].newMaterial;

            crystal.crystalEffect.SetActive(true);
            ParticleSystem particles = crystal.crystalEffect.GetComponent<ParticleSystem>();

            //Setting the colour of the crystal's particle effect
            var pfxMain = particles.main;
            pfxMain.startColor = crystalColours[crystalIndex].particleColor;
            particles.Play();
        }

        //Setting relevant buttons active and inactive
        cameraPanButton.SetActive(false);
        crystalActiveButton.SetActive(false);
        crystalInactiveButton.SetActive(true);
        snapShotButton.SetActive(true);
    }

    public void DeactivateCrystals()
    {
        //Disabling crystal effects
        foreach (CrystalMovable crystal in canvasCrystals)
        {
            //Destroy the crystal's light and set it back to it's original material
            //Destroy(crystal.transform.GetComponentInChildren<Light>());
            Destroy(crystal.transform.GetChild(1).gameObject);
            crystal.GetComponent<MeshRenderer>().material = crystalColours[crystal.colourIndex].oldMaterial;
            crystal.crystalEffect.SetActive(false);
        }

        //Setting relevant buttons active and inactive
        crystalActiveButton.SetActive(true);
        crystalInactiveButton.SetActive(false);
        snapShotButton.SetActive(false);
        cameraPanButton.SetActive(true);
    }

    public void OpenGallery(string url)
    {
        Application.OpenURL(url);
    }
}