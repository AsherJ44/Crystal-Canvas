using System;
using System.Collections.Generic;
using UnityEngine;
using static CrystalFloat;

public class CrystalSpawn : MonoBehaviour
{
    float nextCrystal;
    float crystalTimer;

    public float crystalFloatSpeed;
    public float crystalSpawnY;

    public float spawnXRange;
    public float spawnZRange;

    public float spawnMinTime;
    public float spawnMaxTime;

    public List<Material> crystalColours;

    [Serializable]
    public struct Crystal
    {
        public CrystalFloat crystal;
        public float xRotateRate;
        public float yRotateRate;
        public float zRotateRate;
    }

    public List<Crystal> crystalValues = new List<Crystal>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextCrystal = UnityEngine.Random.Range(spawnMinTime, spawnMaxTime);
        crystalTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (crystalTimer > nextCrystal)
        {
            CreateCrystal();
            nextCrystal = UnityEngine.Random.Range(spawnMinTime, spawnMaxTime);
            crystalTimer = 0.0f;
        }
        else { crystalTimer += Time.deltaTime; }
    }

    void CreateCrystal()
    {
        float crystalX = UnityEngine.Random.Range(transform.position.x - spawnXRange, transform.position.x + spawnXRange);
        float crystalZ = UnityEngine.Random.Range(transform.position.z - spawnZRange, transform.position.z + spawnZRange);

        int crystalInt = UnityEngine.Random.Range(0, crystalValues.Count);
        int crystalColourInt = UnityEngine.Random.Range(0, crystalColours.Count);

        Crystal newCrystalValues = crystalValues[crystalInt];
        CrystalFloat newCrystal = Instantiate(crystalValues[crystalInt].crystal, new Vector3(crystalX, crystalSpawnY, crystalZ), this.transform.rotation);
        
        newCrystal.properties.speed = crystalFloatSpeed;
        newCrystal.properties.xRotate = UnityEngine.Random.Range(-(newCrystalValues.xRotateRate), newCrystalValues.xRotateRate);
        newCrystal.properties.yRotate = UnityEngine.Random.Range(-(newCrystalValues.yRotateRate), newCrystalValues.yRotateRate);
        newCrystal.properties.zRotate = UnityEngine.Random.Range(-(newCrystalValues.zRotateRate), newCrystalValues.zRotateRate);

        var renderer = newCrystal.GetComponentInChildren<Renderer>();
        renderer.material = crystalColours[crystalColourInt];

        var motion = newCrystal.GetComponentInChildren<CrystalMovable>();
        motion.colourIndex = crystalColourInt;
    }
}