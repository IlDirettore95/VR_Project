using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartShooter : MonoBehaviour
{
    private Camera _camera;
    public GameObject projectilePrefab;
    public GameObject firePoint;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnGUI()
    {
        int size = 12;
        float posX = _camera.pixelWidth / 2 - size / 4;
        float posY = _camera.pixelHeight / 2 - size / 2;
        GUI.Label(new Rect(posX, posY, size, size), "[     ]");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);

        }
    }
}
