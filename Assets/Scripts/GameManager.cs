using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using static CrystalSpawn;
using System;
using UnityEngine.Experimental.GlobalIllumination;
using static GameManager;
using static UnityEngine.ParticleSystem;
using System.IO;

public class GameManager : MonoBehaviour
{
    [Header("Game Management Objects")]
    [HideInInspector] public List<CrystalMovable> canvasCrystals; //List of all crystals on the canvas, used to turn them on and off
    public GameObject snapShotButton;
    public GameObject uploadButton;
    public GameObject crystalActiveButton;
    public GameObject crystalInactiveButton;
    public GameObject cameraPanButton;
    public GameObject mouseEffect;
    public Button[] buttons;


    [Header("Analytics Tracking")]
    public CameraPan pan;
    public CameraOutputSaver cameraOutputSaver;
    string folderName = "Analytics";

    [Header("Crystal Values")]
    public bool crystalsActive = false;
    public Light crystalLight;
    public AudioClip[] crystalSounds;

    [Serializable]
    public struct CrystalColours
    {
        public Color lightColor;
        public Color particleColor;
        public float intensity;
        public Material newMaterial;
        public Material oldMaterial;
    }

    public float specialEffectDistanceThreshold;

    [Serializable]
    public struct CrystalSpecialEffects
    {
        public CrystalMovable[] requiredCrystals;
        public ParticleSystem particles;
        public Light lightEffect;
        public float foundCrystals;
    }

    public List<CrystalColours> crystalColours = new List<CrystalColours>();
    public List<CrystalSpecialEffects> specialEffects = new List<CrystalSpecialEffects>();

    public void Start()
    {
        crystalInactiveButton.SetActive(false);
        snapShotButton.SetActive(false);
        uploadButton.SetActive(false);
    }

    public void ActivateCrystals()
    {
        crystalsActive = true;
        mouseEffect.SetActive(true);
        mouseEffect.GetComponent<MouseEffect>().SetPos();

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

            crystal.UpdateLinks();
        }

        //Setting relevant buttons active and inactive
        cameraPanButton.SetActive(false);
        crystalActiveButton.SetActive(false);
        crystalInactiveButton.SetActive(true);
        snapShotButton.SetActive(true);
    }

    public void DeactivateCrystals()
    {
        if (crystalsActive)
        {
            mouseEffect.SetActive(false);
            crystalsActive = false;

            //Disabling crystal effects
            foreach (CrystalMovable crystal in canvasCrystals)
            {
                //Destroy the crystal's light and set it back to it's original material
                Destroy(crystal.transform.GetChild(1).gameObject);
                crystal.GetComponent<MeshRenderer>().material = crystalColours[crystal.colourIndex].oldMaterial;
                crystal.crystalEffect.SetActive(false);

                crystal.WipeLinks();
            }
        }

        //Setting relevant buttons active and inactive
        crystalActiveButton.SetActive(true);
        crystalInactiveButton.SetActive(false);
        snapShotButton.SetActive(false);
        cameraPanButton.SetActive(true);
        uploadButton.SetActive(false);
    }

    /*
    private void SpecialEffectCheck(CrystalMovable crystal)
    {
        List<CrystalMovable> closeCrystals = new List<CrystalMovable>();

        foreach (CrystalMovable nextCrystal in canvasCrystals)
        {
            if (Vector3.Distance(crystal.transform.position, nextCrystal.transform.position) < 0.5f) { Debug.Log(""); }
        }

        foreach (CrystalSpecialEffects effect in specialEffects)
        {
            foreach (CrystalMovable checkCrystal in effect.requiredCrystals)
            {
                if (closeCrystals.Contains(checkCrystal))
                {
                    effect.foundCrystals++;
                }
            }
            if (effect.foundCrystals > effect.requiredCrystals.Count)
            {
                var effectParticles = Instantiate(effect.particles, crystal.transform.position, new Vector3(0,0,0), crystal);
                var effectLight = Instantiate(effect.lightEffect, crystal.transform.position, new Vector3(0, 0, 0), crystal);
            }
        }
    }
    */

    public void OpenGallery(string url)
    {
        Application.OpenURL(url);
    }

    public void ExitGame()
    {
        float timeMinutes;
        if (Time.time > 60) { timeMinutes = (Time.time / 60); }
        else { timeMinutes = 0; }

        float timeSeconds = Mathf.RoundToInt(Time.time % 60);

        string path = Path.Combine(Application.dataPath, folderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string fileName = string.Format("{0}/PlayerAnalytics.txt", path);

        var sr = File.CreateText(fileName);
        sr.WriteLine("Playtime {0} minutes and {1} seconds", timeMinutes, timeSeconds);
        sr.WriteLine("User accessed crystal stream {0} times", pan.timesPanned);
        sr.WriteLine("User took a snapshot {0} times", GlobalInstanceManager.Instance.CameraCaptureIndex);
        sr.Close();
        Application.Quit();
    }
}