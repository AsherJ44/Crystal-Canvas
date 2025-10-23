using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static CrystalSpawn;
using static UnityEngine.GraphicsBuffer;

public class CrystalMovable : MonoBehaviour
{
    //Management values
    [HideInInspector] public GameManager manager;
    [HideInInspector] public bool onCanvas = false;
    [HideInInspector] public bool inDestructionArea = false; //Bool to determine if a crystal is going to be destroyed
    [HideInInspector] public int colourIndex;

    //Bounding values
    float minXBound = -0.195f;
    float maxXBound = 0.195f;
    float minYBound = -0.087f;
    float maxYBound = 0.105f;
    float mouseClickTime = 0.2f;
    float mouseClickTimer;

    [Header("Crystal Values")]
    public bool clickable;
    public GameObject crystalEffect;
    public List<Material> crystalColours; //List of possible crystal colours
    public AudioSource crystalAudio;
    public Animator animator;
    [HideInInspector] public bool reset = false;
    GameObject bin;
    Bin binVoid;
    float mouseDragTime = 0f;
    public AudioClip discardSound;

    [HideInInspector] public bool discarding;

    Vector3 mousePosition;

    [Header("Crystal Connections")]
    //Crystal Connection values
    public int connectionLimit = 2;
    public GameObject crystalConnectEffectOff;
    public GameObject crystalConnectEffectLit;
    //public Animator connectAnimator;

    GameObject crystalConnectEffect;
    [HideInInspector] public CrystalMovable[] connectedCrystals;
    [HideInInspector] public GameObject[] connectEffects;
    Vector3 scaleChange;

    //Crystal Float values
    bool clickedAndMoving = false;
    float lerpLevel = 0.0f;
    Vector3 startPos;
    Vector3 canvasPos;
    bool waiting = false;

    public struct CrystalMotionProperties
    {
        public float speed;
        public float xRotate;
        public float yRotate;
        public float zRotate;
    }

    public CrystalMotionProperties properties = new CrystalMotionProperties();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
        GetComponent<Renderer>().material = crystalColours[colourIndex];
        connectEffects = new GameObject[connectionLimit];
        bin = GameObject.FindGameObjectWithTag("Bin");
        binVoid = manager.binVoid;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(properties.xRotate * Time.deltaTime, properties.yRotate * Time.deltaTime, properties.zRotate * Time.deltaTime, Space.Self);
        if (!onCanvas)
        {
            transform.position = new Vector3(transform.position.x, this.transform.position.y - (properties.speed * Time.deltaTime), transform.position.z);
            
            //Destroys the crystal once it goes low enough
            if (transform.position.y < -0.45f)
            {
                Destroy(gameObject);
            }

            //Moves the crystal over to the canvas and waits for 2 seconds before activating the crystal movable component
            if (clickedAndMoving)
            {
                transform.position = Vector3.Lerp(startPos, canvasPos, lerpLevel);
                lerpLevel += Time.deltaTime;

                if (!waiting)
                {
                    waiting = true;
                    StartCoroutine(WaitToActivate());
                }
            }
        }

        if (reset)
        {
            transform.position = Vector3.Lerp(startPos, bin.transform.position, lerpLevel);
            lerpLevel += Time.deltaTime;

            if (transform.position == bin.transform.position)
            {
                discarding = true;
                manager.canvasCrystals.Remove(this);
                animator.enabled = true;
                crystalAudio.PlayOneShot(discardSound);
                StartCoroutine(WaitAndDestroy());
            }
        }
    }

    private IEnumerator WaitToActivate()
    {
        yield return new WaitForSeconds(2.0f);
        manager.canvasCrystals.Add(this);
        onCanvas = true;
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
        if (!onCanvas && clickable)
        {
            if (manager != null)
            { 
                //Picks a random audio clip from the sounds in manager and plays it
                crystalAudio.clip = manager.crystalStreamSounds[Random.Range(0, manager.crystalStreamSounds.Length)];
                crystalAudio.Play();
            }
            
            //Store a reference of the crystal's current position
            startPos = transform.position;
            //Set random position within the canvas bounds
            float canvasY = UnityEngine.Random.Range(-0.087f, 0.105f);
            float canvasZ = UnityEngine.Random.Range(-0.195f, 0.195f);
            bool posValid = false;
            
            while (!posValid)
            {
                canvasY = UnityEngine.Random.Range(-0.087f, 0.105f);
                canvasZ = UnityEngine.Random.Range(-0.195f, 0.195f);
                if (!(canvasY < 0.105f && canvasY > 0.07f && canvasZ < -0.1575f && canvasZ > -0.195f) && 
                   (!(canvasY < 0.105f && canvasY > 0.08f && canvasZ < -0.17f && canvasZ > -0.195f)) && 
                   (!(canvasY < -0.087f && canvasY > 0.065f && canvasZ < 0.167f && canvasZ > 0.195f)))  { posValid = true; break; }
            }
            
            canvasPos = new Vector3(-0.25f, canvasY, canvasZ);

            //Setting the crystal to start lerping over to the canvas
            clickedAndMoving = true;
            properties.speed = 0;
        }
        
        if (onCanvas && !manager.crystalsActive && clickable)
        {
            binVoid.CrystalPickedUp();

            //Disables the buttons while the player is moving a crystal
            foreach (Button button in manager.buttons)
            {
                if (button != null) { button.gameObject.SetActive(false); }
            }

            //Picks a random audio clip from the sounds in manager and plays it
            crystalAudio.clip = manager.crystalSounds[Random.Range(0, manager.crystalSounds.Length)];
            crystalAudio.Play();

            mousePosition = Input.mousePosition - GetMousePosition();
            mouseClickTimer = Time.time + mouseClickTime; //Starting the mouse click timer

            if (manager.canvasCrystals.Count > 1)
            {
                connectedCrystals = FindNearestCrystals();
                for (int i = 0; i < connectedCrystals.Length; i++)
                {   
                    if (connectEffects[i] == null)
                    {
                        connectEffects[i] = Instantiate(crystalConnectEffectOff, new Vector3(0, 0, 0), transform.rotation);
                    }
                }
            }
        }
    }

    private void OnMouseDrag()
    {
        if (onCanvas && !manager.crystalsActive)
        {
            if (mouseDragTime > 1f) { binVoid.CrystalPickedUp(); }
            else { mouseDragTime += Time.deltaTime; }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        
            //Ensuring the crystal stays within the screen bounds even if the player attempts to move it outside of them
            if (mousePos.z < minXBound) { mousePos.z = minXBound; }
            else if (mousePos.z > maxXBound) { mousePos.z = maxXBound; }

            if (mousePos.y < minYBound) { mousePos.y = minYBound; }
            else if (mousePos.y > maxYBound) { mousePos.y = maxYBound; }

            transform.position = mousePos; //Moving the object

            UpdateLinks();
        }
    }

    private void OnMouseUp()
    {
        if (onCanvas)
        {
            if (!manager.crystalsActive)
            {
                //Re-enables the key buttons
                foreach (Button button in manager.buttons)
                {
                    button.gameObject.SetActive(true);
                }
            }

            mouseDragTime = 0f;
            binVoid.CrystalDropped();

            //Picks a random audio clip from the sounds in manager and plays it
            crystalAudio.clip = manager.crystalSounds[Random.Range(0, manager.crystalSounds.Length)];
            crystalAudio.Play();

            if (inDestructionArea) 
            {
                discarding = true;
                manager.canvasCrystals.Remove(this);
                animator.enabled = true;
                crystalAudio.PlayOneShot(discardSound);
                StartCoroutine(WaitAndDestroy());
            }

            if (!manager.crystalsActive)
            {
                WipeLinks();
            }
        }
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
                if (crystal.enabled && !crystal.discarding) //Checks the crystal exists and is not in the process of being destroyed
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

            int crystalCount = connectionLimit;
            if (distSorted.Count < connectionLimit) { crystalCount = distSorted.Count; }

            CrystalMovable[] tempCrystals = new CrystalMovable[crystalCount];

            for (int i = 0; i < crystalCount; i++)
            {
                tempCrystals[i] = distSorted[i];
            }

            return tempCrystals;
        }

        //Returning null if no other crystals are on the canvas
        return null;
    }

    //Spawns links between a crystal and all other relevant crystals
    public void UpdateLinks()
    {
        connectedCrystals = FindNearestCrystals();
        if (connectedCrystals != null)
        {
            for (int i = 0; i < connectedCrystals.Length; i++)
            {
                if (connectEffects[i] == null)
                {
                    if (manager.crystalsActive) { crystalConnectEffect = crystalConnectEffectLit; }
                    else { crystalConnectEffect = crystalConnectEffectOff; }
                    connectEffects[i] = Instantiate(crystalConnectEffect);
                }
                
                ConnectEffect(this, connectedCrystals[i], connectEffects[i]);
            }
        }
    }

    //Destroys all the connections of a given crystal
    public void WipeLinks()
    {
        foreach (GameObject connection in this.connectEffects)
        {
            Destroy(connection);
        }
    }

    public void ConnectEffect(CrystalMovable thisCrystal, CrystalMovable connectedCrystal, GameObject connectEffect)
    {
        Vector3 effectPos = Vector3.Lerp(thisCrystal.transform.position, connectedCrystal.transform.position, 0.5f);

        float crystalDistance = Vector3.Distance(thisCrystal.transform.position, connectedCrystal.transform.position);

        connectEffect.transform.position = effectPos;

        if (manager.crystalsActive) { scaleChange = new Vector3(connectEffect.transform.localScale.x, connectEffect.transform.localScale.x, crystalDistance * 1.5f);}
        else { scaleChange = new Vector3(connectEffect.transform.localScale.x, connectEffect.transform.localScale.x, crystalDistance); }
       
        connectEffect.transform.localScale = scaleChange;

        connectEffect.transform.LookAt(new Vector3(connectEffect.transform.position.x, connectedCrystal.transform.position.y, connectedCrystal.transform.position.z));
    }

    public void FlyAndDie()
    {
        lerpLevel = 0f;
        reset = true;
        startPos = transform.position;
    }
}