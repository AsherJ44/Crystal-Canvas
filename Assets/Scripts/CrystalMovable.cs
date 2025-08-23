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

    Vector3 mousePosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material = crystalColours[colourIndex];
    }

    // Update is called once per frame
    void Update()
    {

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
        mouseClickTimer = Time.time + mouseClickTime;
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }

    private void OnMouseUp()
    {
        if (Time.time < mouseClickTimer) { ColourCycle(); }
    }

    private void ColourCycle()
    {
        colourIndex++;
        if (colourIndex >= crystalColours.Count) { colourIndex = 0; } //Handling for looping back to the first list element
        
        //Getting the renderer component and setting the colour to the next available colour
        var renderer = GetComponent<Renderer>();
        renderer.material = crystalColours[colourIndex];
    }
}