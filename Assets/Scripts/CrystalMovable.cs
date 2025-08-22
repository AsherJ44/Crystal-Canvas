using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalMovable : MonoBehaviour
{
    [SerializeField] private InputAction colourChange; //Binding to change crystal colour

    [SerializeField] private InputAction mouseClick; //Crystal selection binding

    public List<Material> crystalColours; //List of possible crystal colours

    [HideInInspector] public bool IsMoving = false; //Bool for movement

    //[HideInInspector] 
    public int colourIndex;

    public float mouseClickTime = 0.2f;
    public float mouseClickTimer;

    new Renderer renderer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material = crystalColours[colourIndex];
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (IsMoving) 
        {
            transform.position = GetMousePosition();
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mouseInput = Mouse.current.position.ReadValue();
        mouseInput.z = transform.position.z - Camera.main.transform.position.z;
        
        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mouseInput);
        mouseInWorld.x = transform.position.x;
        return mouseInWorld;
    }

    private void OnMouseDown()
    {
        IsMoving = true;
        //ColourCycle();
        mouseClickTimer = Time.time;
    }

    private void OnMouseDrag()
    {
        //transform.position = GetMousePosition();
        IsMoving = true;
    }

    private void OnMouseUp()
    {
        IsMoving = false;
        if (mouseClickTimer < Time.time - mouseClickTime) { ColourCycle(); }
    }

    /*
    private void MouseClickAction(InputAction.CallbackContext context)
    {
        //Get mouse pos
        Vector3 mouseInWorld = GetMousePosition();

        //Check if mouse over crystal
        
    }
    

    private void MouseReleaseAction(InputAction.CallbackContext context)
    {
        IsMoving = false;
    }
   
    Code for new input system, needs raycasts which is annoying
    private void OnEnable()
    {
        colourChange.Enable();
        colourChange.performed += ColourCycle;

        
        mouseClick.Enable();
        mouseClick.performed += MouseClickAction;
        mouseClick.canceled += MouseReleaseAction;
    }

    private void OnDisable()
    {
        colourChange.Disable();
        colourChange.performed -= ColourCycle;

        mouseClick.Disable();
        mouseClick.performed -= MouseClickAction;
        mouseClick.canceled -= MouseReleaseAction;
        
    }

    private void ColourCycle(InputAction.CallbackContext context)
    {
        colourIndex++;
        if (colourIndex >= crystalColours.Count) { colourIndex = 0; } //Handling for looping back to the first list element
        
        //Getting the renderer component and setting the colour to the next available colour
        var renderer = GetComponent<Renderer>();
        renderer.material = crystalColours[colourIndex];
    }
    */

    private void ColourCycle()
    {
        colourIndex++;
        if (colourIndex >= crystalColours.Count) { colourIndex = 0; } //Handling for looping back to the first list element
        
        //Getting the renderer component and setting the colour to the next available colour
        var renderer = GetComponent<Renderer>();
        renderer.material = crystalColours[colourIndex];
    }
}