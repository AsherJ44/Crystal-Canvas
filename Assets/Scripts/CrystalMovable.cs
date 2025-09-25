using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static CrystalSpawn;
using static UnityEngine.GraphicsBuffer;

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
    public AudioSource crystalAudio;

    public AnimationCurve bobCurve;
    bool bobbing = false;
    public float maxBobTime;
    float bobTime;

    Vector3 mousePosition;

    public Animator animator;

    //Crystal Connection values
    [HideInInspector] public CrystalMovable[] connectedCrystals;
    public int connectionLimit = 2;
    public GameObject crystalConnectEffect;
    GameObject[] connectEffects;
    Animator connectAnimator;

    void OnEnable()
    {
        //Setting the crystal floating element to inactive once the crystal is made movable
        CrystalFloat crystalFloat = GetComponent<CrystalFloat>();
        crystalFloat.enabled = false;
        bobbing = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
        GetComponent<Renderer>().material = crystalColours[colourIndex];
        connectEffects = new GameObject[connectionLimit];
    }

    /*
    void Update()
    {
        if (bobbing)
        {
            if (bobTime > maxBobTime) { bobTime = 0; }
            transform.position = new Vector3(transform.position.x, transform.position.y + bobCurve.Evaluate(bobTime), transform.position.z);
            bobTime += Time.deltaTime;
        }
    }
    */
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
        if (this.enabled)
        {
            bobbing = false;
            //Picks a random audio clip from the sounds in manager and plays it
            crystalAudio.clip = manager.crystalSounds[Random.Range(0, manager.crystalSounds.Length)];
            crystalAudio.Play();

            mousePosition = Input.mousePosition - GetMousePosition();
            mouseClickTimer = Time.time + mouseClickTime; //Starting the mouse click timer

            for (int i = 0; i < connectionLimit; i++)
            {
                 Destroy(connectEffects[i]);
            }

            for (int i = 0; i < connectionLimit; i++)
            {
                connectEffects[i] = Instantiate(crystalConnectEffect, new Vector3(0, 0, 0), transform.rotation);
            }
        }
    }

    private void OnMouseDrag()
    {
        if (this.enabled)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        
            //Ensuring the crystal stays within the screen bounds even if the player attempts to move it outside of them
            if (mousePos.z < minXBound) { mousePos.z = minXBound; }
            else if (mousePos.z > maxXBound) { mousePos.z = maxXBound; }

            if (mousePos.y < minYBound) { mousePos.y = minYBound; }
            else if (mousePos.y > maxYBound) { mousePos.y = maxYBound; }

            transform.position = mousePos; //Moving the object

            connectedCrystals = FindNearestCrystals();
            if (connectedCrystals != null) 
            {
                for (int i = 0; i < connectionLimit; i++)
                {
                    ConnectEffect(connectedCrystals[i], connectEffects[i]);
                }
            }
        }
    }

    private void OnMouseUp()
    {
        //If the mouse is down for less than 0.2 seconds, change the colour
        //if (Time.time < mouseClickTimer && moveComplete) { ColourCycle(); }
        if (this.enabled)
        {
            bobbing = true;
            //Picks a random audio clip from the sounds in manager and plays it
            crystalAudio.clip = manager.crystalSounds[Random.Range(0, manager.crystalSounds.Length)];
            crystalAudio.Play();

            if (inDestructionArea) 
            {
                manager.canvasCrystals.Remove(this);
                animator.enabled = true;
                //animator.SetBool("FlyingAway", true);
                StartCoroutine(WaitAndDestroy());
            }

            for (int i = 0; i < connectionLimit; i++)
            {
                Destroy(connectEffects[i]);
            }

            if (manager.crystalsActive)
            {
                manager.UpdateLinks();
            }
        }

        //Add code to calculate direction and velocity of crystal current position compared to previous position
    }

    IEnumerator WaitAndDestroy()
    {
        this.enabled = false; //Disabling while flying away to prevent erroneous connections
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

    private CrystalMovable[] FindNearestCrystals()
    {
        //Iterates through all the crystal movable objects and returns the closest ones, up to the connection limit of the crystal

        if (manager.canvasCrystals.Count > 1)
        {
            float closestCrystal = 9999f;
            List<CrystalMovable> distSorted = new List<CrystalMovable>();

            foreach (CrystalMovable crystal in manager.canvasCrystals)
            {
                if (crystal.enabled)
                {
                    float crystalDistance = Vector3.Distance(transform.position, crystal.transform.position);

                    //If the crystal isn't the current crystal and is closer than the other closest crystal
                    if (crystal.transform != this.transform)
                    {
                        if(crystalDistance < closestCrystal)
                        {
                            distSorted.Insert(0, crystal);
                            closestCrystal = crystalDistance;
                        }
                        else { distSorted.Add(crystal); }
                    }
                }
            }

            CrystalMovable[] tempCrystals = new CrystalMovable[connectionLimit];
            for (int i = 0; i < tempCrystals.Length; i++)
            {
                tempCrystals[i] = distSorted[i];
            }

            return tempCrystals;
        }

        //Returning null if no other crystals are on the canvas
        return null;
    }

    private void ConnectEffect(CrystalMovable connectedCrystal, GameObject connectEffect)
    {
        Vector3 effectPos = Vector3.Lerp(transform.position, connectedCrystal.transform.position, 0.5f);

        float crystalDistance = Vector3.Distance(transform.position, connectedCrystal.transform.position);

        connectEffect.transform.position = effectPos;

        Vector3 scaleChange = new Vector3(connectEffect.transform.localScale.x, connectEffect.transform.localScale.x, crystalDistance);
        connectEffect.transform.localScale = scaleChange;
        //ParticleSystem connectParticles = connectEffect.GetComponentInChildren<ParticleSystem>();
        //var shape = connectParticles.shape;
        //shape.scale = new Vector3(shape.scale.x, shape.scale.y, (crystalDistance * 0.5f));

        connectEffect.transform.LookAt(new Vector3(connectEffect.transform.position.x, connectedCrystal.transform.position.y, connectedCrystal.transform.position.z));
    }
}