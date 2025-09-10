using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static CrystalSpawn;

public class CrystalMovable : MonoBehaviour
{
    [HideInInspector] public GameManager manager;

    float minXBound = -0.195f;
    float maxXBound = 0.195f;
    float minYBound = -0.087f;
    float maxYBound = 0.105f;

    [HideInInspector] public bool moveComplete = false;

    public GameObject crystalEffect;

    public List<Material> crystalColours; //List of possible crystal colours

    public bool inDestructionArea = false; //Bool to determine if a crystal is going to be destroyed

    [HideInInspector] public int colourIndex;

    private float mouseClickTime = 0.2f;
    private float mouseClickTimer;

    new Renderer renderer;

    Vector3 mousePosition;

    public Animator animator;

    //Crystal Connection values
    //[HideInInspector] 
    public CrystalMovable connectedCrystal;
    bool canConnect = false;
    public GameObject crystalConnectEffect;
    GameObject connectEffect;

    void OnEnable()
    {
        //Setting the crystal floating element to inactive once the crystal is made movable
        CrystalFloat crystalFloat = GetComponent<CrystalFloat>();
        crystalFloat.enabled = false;
        animator.enabled = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
        renderer = GetComponent<Renderer>();
        renderer.material = crystalColours[colourIndex];
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.transform.CompareTag("Bin"))
        {
            inDestructionArea = true;
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other)
    {
        if (other.transform.CompareTag("Bin"))
        {
            inDestructionArea = false;
        }
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
        connectEffect = Instantiate(crystalConnectEffect, new Vector3(0, 0, 0), transform.rotation);
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
        ConnectEffect();
    }

    private void OnMouseUp()
    {
        //If the mouse is down for less than 0.2 seconds, change the colour
        //if (Time.time < mouseClickTimer && moveComplete) { ColourCycle(); }

        if (inDestructionArea) 
        {
            manager.canvasCrystals.Remove(this);
            animator.SetBool("FlyingAway", true);
            StartCoroutine(WaitAndDestroy());
        }

        Destroy(connectEffect);

        //Add code to calculate direction and velocity of crystal current position compared to previous position
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    private void ColourCycle()
    {
        colourIndex++;
        if (colourIndex >= crystalColours.Count) { colourIndex = 0; } //Handling for looping back to the first list element
        
        //Getting the renderer component and setting the colour to the next available colour
        var renderer = GetComponent<Renderer>();
        renderer.material = crystalColours[colourIndex];
    }

    private CrystalMovable FindNearestCrystal()
    {
        //Iterates through all the crystal movable objects and returns the closest one
        CrystalMovable[] crystals = FindObjectsByType<CrystalMovable>(FindObjectsSortMode.None);
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
        Vector3 effectPos = Vector3.Lerp(transform.position, connectedCrystal.transform.position, 0.5f);
        //float direction = Vector3.SignedAngle(transform.position, connectedCrystal.transform.position, Vector3.up);

        float crystalDistance = Vector3.Distance(transform.position, connectedCrystal.transform.position);

        connectEffect.transform.position = effectPos;

        Vector3 scaleChange = new Vector3(0.01f, 0.01f, crystalDistance);
        connectEffect.transform.localScale = scaleChange;
        ParticleSystem connectParticles = connectEffect.GetComponentInChildren<ParticleSystem>();
        var shape = connectParticles.shape;
        shape.scale = new Vector3(0.5f, 0.5f, (crystalDistance * 0.5f));

        connectEffect.transform.LookAt(connectedCrystal.transform.position);
    }
}