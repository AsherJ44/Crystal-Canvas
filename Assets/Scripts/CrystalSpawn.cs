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
    public float crystalSpawnZ;

    public List<Shader> crystalColours;

    [Serializable]
    public struct Crystal
    {
        public CrystalFloat crystal;
        public float minXRotate;
        public float maxXRotate;
        public float minYRotate;
        public float maxYRotate;
        public float minZRotate;
        public float maxZRotate;
    }

    public List<Crystal> crystalValues = new List<Crystal>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextCrystal = UnityEngine.Random.Range(0.5f, 1.5f);
        crystalTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (crystalTimer > nextCrystal)
        {
            CreateCrystal();
            nextCrystal = UnityEngine.Random.Range(0.5f, 1.5f);
            crystalTimer = 0.0f;
        }
        else { crystalTimer += Time.deltaTime; }
    }

    void CreateCrystal()
    {
        float crystalX = UnityEngine.Random.Range(-0.05f, 0.05f);
        int crystalInt = UnityEngine.Random.Range(0, crystalValues.Count);
        int crystalColourInt = UnityEngine.Random.Range(0, crystalColours.Count);
        Crystal newCrystalValues = crystalValues[crystalInt];
        CrystalFloat newCrystal = Instantiate(crystalValues[crystalInt].crystal, new Vector3(crystalX, crystalSpawnY, crystalSpawnZ), this.transform.rotation);
        
        newCrystal.properties.speed = crystalFloatSpeed;
        newCrystal.properties.xRotate = UnityEngine.Random.Range(newCrystalValues.minXRotate, newCrystalValues.maxXRotate);
        newCrystal.properties.yRotate = UnityEngine.Random.Range(newCrystalValues.minYRotate, newCrystalValues.maxYRotate);
        newCrystal.properties.zRotate = UnityEngine.Random.Range(newCrystalValues.minZRotate, newCrystalValues.maxZRotate);

        var renderer = newCrystal.GetComponent<Renderer>();
        renderer.material.shader = crystalColours[crystalColourInt];
    }
}