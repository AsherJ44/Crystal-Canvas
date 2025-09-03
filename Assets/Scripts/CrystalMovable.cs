using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalMovable : MonoBehaviour
{
    float minXBound = -0.195f;
    float maxXBound = 0.195f;
    float minYBound = -0.087f;
    float maxYBound = 0.105f;

    [HideInInspector] public bool moveComplete = false;

    public GameObject crystalEffect;

    public List<Material> crystalColours; //List of possible crystal colours

    [HideInInspector] public bool IsMoving = false; //Bool for movement

    //[HideInInspector]
    public int colourIndex;

    private float mouseClickTime = 0.2f;
    private float mouseClickTimer;

    new Renderer renderer;

    Vector3 mousePosition;

    //[HideInInspector] 
    public CrystalMovable connectedCrystal;
    bool canConnect = false;

    public GameObject crystalConnectEffect;

    void OnEnable()
    {
        //Setting the crystal floating element to inactive once the crystal is made movable
        CrystalFloat crystalFloat = GetComponent<CrystalFloat>();
        crystalFloat.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material = crystalColours[colourIndex];
    }

    private Vector3 GetMousePosition()
    {
        //Getting the mouse's position in relation to the world space
        Vector3 mouseInWorld = Camera.main.WorldToScreenPoint(transform.position);
        return mouseInWorld;
    }

    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePosition();
        mouseClickTimer = Time.time + mouseClickTime; //Starting the mouse click timer
    }

    private void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        
        //Ensuring the crystal stays within the screen bounds even if the player attempts to move it outside of them
        if (mousePos.z < minXBound) { mousePos.z = minXBound; }
        else if (mousePos.z > maxXBound) { mousePos.z = maxXBound; }

        if (mousePos.y < minYBound) { mousePos.y = minYBound; }
        else if (mousePos.y > maxYBound) { mousePos.y = maxYBound; }

        transform.position = mousePos; //Moving the object

        connectedCrystal = FindNearestCrystal();
        
    }

    private void OnMouseUp()
    {
        if (Time.time < mouseClickTimer && moveComplete) { ColourCycle(); } //If the mouse is down for less than 0.2 seconds, change the colour
    }

    private void ColourCycle()
    {
        colourIndex++;
        if (colourIndex >= crystalColours.Count) { colourIndex = 0; } //Handling for looping back to the first list element
        
        //Getting the renderer component and setting the colour to the next available colour
        var renderer = GetComponent<Renderer>();
        renderer.material = crystalColours[colourIndex];
    }

    public void EffectOn()
    {
        crystalEffect.SetActive(true);
    }

    public void EffectOff()
    { 
        crystalEffect.SetActive(false); 
    }

    private CrystalMovable FindNearestCrystal()
    {   
        //Iterates through all the crystal movable objects and returns the closest one
        CrystalMovable[] crystals = FindObjectsOfType<CrystalMovable>();
        float closestCrystal = 9999f;
        CrystalMovable tempCrystal = null;

        foreach (CrystalMovable crystal in crystals)
        {
            float crystalDistance = Vector3.Distance(transform.position, crystal.transform.position);
            if(crystalDistance < closestCrystal && crystal.transform != this.transform)
            {
                closestCrystal = crystalDistance;
                tempCrystal = crystal;
            }
        }
        return tempCrystal;
    }

    private void ConnectEffect()
    {
        GameObject[] connectObject = GameObject.FindGameObjectsWithTag("ConnectEffect");
        GameObject newEffect = null;

        Vector3 effectPos = Vector3.Lerp(transform.position, connectedCrystal.transform.position, 0.5f);
        float direction = Vector3.SignedAngle(transform.position, connectedCrystal.transform.position, Vector3.up);

        if (connectObject.Length == 0) 
        {
            newEffect = Instantiate(crystalConnectEffect, effectPos, Quaternion.Euler(0, direction, 0));
        }

        else 
        { 
            newEffect = connectObject[0];
            newEffect.transform.position = effectPos;
            newEffect.transform.rotation = Quaternion.Euler(0, direction, 0);
            //newEffect.
        }

        //Vector3 scaleChange = new Vector3(0, 0, Vector3.Distance(transform.position, connectedCrystal.transform.position) - transform.localScale.z);
        //newEffect.transform.localScale += scaleChange;
        
        //newEffect.transform.localScale.z = Vector3.Distance(transform.position, connectedCrystal.transform.position);
        
    }
}