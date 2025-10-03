using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseEffect : MonoBehaviour
{
    Vector3 mousePosition;
    public GameManager manager;

    private void Start()
    {
        manager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (manager.crystalsActive)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = 10f;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            transform.position = mouseWorldPosition;
        }
    }

    public Vector3 SetPos()
    {
        return Input.mousePosition - GetMousePosition();
    }

    private Vector3 GetMousePosition()
    {
        //Getting the mouse position in relation to the world space
        Vector3 mouseInWorld = Camera.main.WorldToScreenPoint(transform.position);
        return mouseInWorld;
    }
}
