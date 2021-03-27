using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartShooter : PlayerWeapon
{
    private Camera _camera;
    public GameObject projectilePrefab;
    public GameObject firePoint;
    private int MAXRELOAD = 30;
    private int serbatoio;
    float fireRate= 20f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        serbatoio = MAXRELOAD;
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
        if (Input.GetButtonDown("Fire1"))
        {
            InvokeRepeating("Shoot", 0f, 1f/fireRate);
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            CancelInvoke("Shoot");
        }

        if (Input.GetKeyDown(KeyCode.R) && !Input.GetButton("Fire1"))
        {
            serbatoio = MAXRELOAD;
            Debug.Log("Reloading!");
            //Debug.Log(serbatoio);
        }
    }

    void Shoot()
    {
        if(serbatoio > 0)
        {
            Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
            serbatoio--;
            Debug.Log(serbatoio);
        }
    }
}
