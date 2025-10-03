using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseEffect : MonoBehaviour
{
    Vector3 mousePosition;
    public GameManager manager;

    private void Start()
    {
        manager = gameObject.GetComponent<GameManager>();
    }

    void Update()
    {
        if (manager.crystalsActive)
        {
            mousePosition = Input.mousePosition - GetMousePosition();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
            transform.position = mousePos;
        }
    }

    private Vector3 GetMousePosition()
    {
        //Getting the mouse position in relation to the world space
        Vector3 mouseInWorld = Camera.main.WorldToScreenPoint(transform.position);
        return mouseInWorld;
    }
}
