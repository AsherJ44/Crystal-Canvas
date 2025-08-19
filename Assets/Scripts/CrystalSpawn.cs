using System;
using System.Collections.Generic;
using UnityEngine;
using static CrystalFloat;

public class CrystalSpawn : MonoBehaviour
{
    public float crystalFloatSpeed;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
