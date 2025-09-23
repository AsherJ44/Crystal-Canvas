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
    public List<GameObject> crystalConnections = new List<GameObject>();

    public void Start()
    {
        crystalInactiveButton.SetActive(false);
        snapShotButton.SetActive(false);
        uploadButton.SetActive(false);
    }

    public void ActivateCrystals()
    {
        crystalsActive = true;

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

            //Find linked crystal and instantiate effect
            if (crystal.connectedCrystal != null) { ConnectEffect(crystal, crystal.connectedCrystal); }
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
            crystalsActive = false;

            //Disabling crystal effects
            foreach (CrystalMovable crystal in canvasCrystals)
            {
                //Destroy the crystal's light and set it back to it's original material
                Destroy(crystal.transform.GetChild(1).gameObject);
                crystal.GetComponent<MeshRenderer>().material = crystalColours[crystal.colourIndex].oldMaterial;
                crystal.crystalEffect.SetActive(false);
            }

            //Destroying all the connection effects between crystals
            foreach(GameObject connection in crystalConnections) { Destroy(connection); }

            crystalConnections = new List<GameObject>();
        }

        //Setting relevant buttons active and inactive
        crystalActiveButton.SetActive(true);
        crystalInactiveButton.SetActive(false);
        snapShotButton.SetActive(false);
        cameraPanButton.SetActive(true);
        uploadButton.SetActive(false);
    }

    private void UpdateLinks()
    {
        foreach(CrystalMovable crystal in canvasCrystals)
        {
            crystal.connectedCrystals = new CrystalMovable[crystal.connectionLimit];

            //Loops through the list of crystals and checks they aren't the same as the current one
            foreach (CrystalMovable otherCrystal in canvasCrystals)
            {
                if (otherCrystal != crystal)
                {
                    float crystalGap = Vector3.Distance(crystal.transform.position, otherCrystal.transform.position);
                    bool crystalAdded = false;
                    int biggestDistanceIndex = 999;
                    for(int i = 0; i < crystal.connectionLimit; i++)
                    {
                        //If the crystal has an empty connection, adding this one regardless of the distance
                        if (crystal.connectedCrystals[i] = null) { crystal.connectedCrystals[i] = otherCrystal; crystalAdded = true; break; }

                        float tempGap = Vector3.Distance(crystal.transform.position, crystal.connectedCrystals[i].transform.position);
                        //If the crystal has a current connection, check the distance between the current connection and the possible new one, and take note if it's longer
                        if (tempGap > crystalGap)
                        {
                            crystalGap = tempGap;
                            biggestDistanceIndex = i;
                        }
                    }
                    
                    //If the crystal was closer than one of the currently connected ones, add it to the list
                    if (!crystalAdded && biggestDistanceIndex != 999) { crystal.connectedCrystals[biggestDistanceIndex] = otherCrystal; }
                }
            }
        }
    }

    private void ConnectEffect(CrystalMovable crystal1, CrystalMovable crystal2)
    {
        Vector3 effectPos = Vector3.Lerp(crystal1.transform.position, crystal2.transform.position, 0.5f);

        float crystalDistance = Vector3.Distance(crystal1.transform.position, crystal2.transform.position);

        GameObject connectEffect = Instantiate(crystal1.crystalConnectEffect, crystal1.transform);

        connectEffect.transform.position = effectPos;

        Vector3 scaleChange = new Vector3(connectEffect.transform.localScale.x, connectEffect.transform.localScale.y, crystalDistance);
        connectEffect.transform.localScale = scaleChange;
        ParticleSystem connectParticles = connectEffect.GetComponentInChildren<ParticleSystem>();
        var shape = connectParticles.shape;
        shape.scale = new Vector3(shape.scale.x, shape.scale.y, (crystalDistance * 0.5f));

        connectEffect.transform.LookAt(crystal2.transform.position);

        crystalConnections.Add(connectEffect);
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
        sr.WriteLine("User took a snapshot {0} times", cameraOutputSaver.captureIndex);
        sr.Close();
        Application.Quit();
    }
}