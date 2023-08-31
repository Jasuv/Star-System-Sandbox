using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int scrollSpeed;
    public float moveSensitivity;
    public GameObject bodyConsole;
    public GameObject selectedUI;
    public GameObject selected;
    public bool isMoving;
    
    private float zoom;
    private Camera cam;
    private Vector3 lastPosition;

    void Start()
    {
        cam = GetComponent<Camera>();
        zoom = cam.orthographicSize;
    }

    void Update()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * 1000;
        zoom = Mathf.Clamp(zoom, 400, 8000);
        cam.orthographicSize = zoom;

        if (Input.GetMouseButtonDown(1))
        {
            lastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(-delta.x * (moveSensitivity * 2.2f) * (zoom / 1000), -delta.y * (moveSensitivity * 2.2f) * (zoom / 1000), 0);
            lastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                selected = hit.transform.gameObject;
                selectedUI = selected;
            }
        }

        if (selected != null)
        {
            bodyConsole.SetActive(true);
            isMoving = true;
            Vector3 mousePos = Input.mousePosition;
            Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
            selected.transform.position = new Vector3(Worldpos.x, 0, Worldpos.z);
        }

        if (selectedUI != null) 
            bodyConsole.SetActive(true);
        else
            bodyConsole.SetActive(false);

        if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
            selected = null;
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
