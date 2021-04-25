using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : PlayerWeapon
{
    //In this first version the shooter script has only the gravity gun implementation
    private Camera _camera;

    //Gravity gun status
    private bool isAttracting = false;
    private bool isReleasing = false;
    private bool isLaunching = false;
    private bool isRepulsing = false;
    private ReactiveObject _reactiveComponent;
    public Transform floatingPoint; //Must be set from the inspector
    public float repulsingSpeed;
    public float repulsingRange;
    public float attractionSpeed;
    public float attractionRange;
    public float launchingSpeed;

    // Start is called before the first frame update
    void Start()
    {
       
        _camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAttracting)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
                Ray ray = _camera.ScreenPointToRay(point);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, attractionRange))
                {
                    GameObject hitObject = hit.transform.gameObject;
                    ReactiveObject target = hitObject.GetComponent<ReactiveObject>();
                    if (target != null)
                    {
                        _reactiveComponent = target;
                        isAttracting = true;
                    }
                }
            }
            else if(Input.GetButtonDown("Fire2"))
            {
                Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
                Ray ray = _camera.ScreenPointToRay(point);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, repulsingRange))
                {
                    GameObject hitObject = hit.transform.gameObject;
                    ReactiveObject target = hitObject.GetComponent<ReactiveObject>();
                    if (target != null)
                    {
                        _reactiveComponent = target;
                        isAttracting = false;
                        isRepulsing = true;
                    }
                }
            }
        }
        else if(isAttracting)
        {
            //_reactiveComponent is surely != null
            if(Input.GetButtonUp("Fire1"))
            {
                isAttracting = false;
                isReleasing = true;
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                isAttracting = false;
                isLaunching = true;
            }
        }
        
    }

    private void FixedUpdate()
    {
        if(isAttracting)
        {
            _reactiveComponent.ReactToAttraction(floatingPoint.transform.position, attractionSpeed);
        }
        else if(isRepulsing)
        {
            _reactiveComponent.ReactToRepulsing(floatingPoint.transform.forward, repulsingSpeed);
            _reactiveComponent = null;
            isRepulsing = false;
        }
        else if(isReleasing)
        {
            _reactiveComponent.ReactToReleasing();
            _reactiveComponent = null;
            isReleasing = false;
        }
        else if(isLaunching)
        {
            _reactiveComponent.ReactToLaunching(floatingPoint.transform.forward, launchingSpeed);
            _reactiveComponent = null;
            isLaunching = false;
        }
    }

    private void OnGUI()
    {
        int size = 24;
        float posX = _camera.pixelWidth / 2 - size / 4;
        float posY = _camera.pixelHeight / 2 - size / 2;

        GUI.Label(new Rect(posX, posY, size, size), "[    ]");
    }
}
