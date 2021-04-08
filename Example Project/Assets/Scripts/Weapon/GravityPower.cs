using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class handles the player gravity powers
 * Attraction:
 * Releasing: 
 * Launching: launching an entity while attracted
 * Repulsing: 
 * Increase entity mass:
 * Decrease entity mass:
 * Nailgun:
 */
public class GravityPower : MonoBehaviour
{
    //Keycodes
    public KeyCode attractionKey;
    public KeyCode launchingKey;
    public KeyCode increaseKey;
    public KeyCode decreaseKey;
    public KeyCode shootingKey;

    //GravityPowers status
    private bool attracting = false;
    private bool releasing = false;
    private bool launching = false;
    private bool increasing = false;
    private bool decreasing = false;
    private bool shooting = false;

    //ReactiveObject target
    private ReactiveObject target;

    //Speeds
    public float attractionSpeed;
    public float launchingSpeed;
    public float shootingSpeed;

    //Aux methods for external questioning
    public bool IsAttracting() => attracting;
    public bool IsReleasing() => releasing;
    public bool IsLaunching() => launching;
    public bool IsIncreasing() => increasing;
    public bool IsDecreasing() => decreasing;
    public bool IsShooting() => shooting;

    private Camera _camera;

    private GUIStyle style = new GUIStyle();

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        style.fontSize = 24;
        style.normal.textColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(attracting);
        if (!shooting)
        {
            if(!attracting)
            {
                if (Input.GetKeyDown(attractionKey)) Attraction();
                    
                else if (Input.GetKeyDown(shootingKey)) Shooting();
            }
            else
            {
                if (Input.GetKeyDown(launchingKey)) Launching();

                else if (Input.GetKeyDown(increaseKey)) Increasing();

                else if (Input.GetKeyDown(decreaseKey)) Decreasing();

                else if (Input.GetKeyDown(attractionKey)) Releasing();
            }
        }
    }

    private void FixedUpdate()
    {
        if (attracting)
        {
            target.ReactToAttraction(attractionSpeed);
        }
        if(releasing)
        {
            target.ReactToReleasing();
            target = null;
            releasing = false;
            attracting = false;
        }
        if(launching)
        {
            target.ReactToLaunching(launchingSpeed);
            target = null;
            launching = false;
            attracting = false;
        }

        
    }

    private void Attraction()
    { 
        target = AcquireTarget();
        if (target == null)
        {
            //Default action
            return;
        }
        else
        {
            attracting = true;
        }
    }

    private void Releasing()
    {
        
        releasing = true;
    }

    private void Launching()
    {
        Debug.Log("AIUTO");
        launching = true;
        attracting = false;
    }

    private void Increasing() => increasing = true;

    private void Decreasing() => decreasing = true;

    private void Shooting()
    {
        //Shoot
    }

    //Acquiring the target gameobject or null
    private ReactiveObject AcquireTarget()
    {
        Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) return hit.transform.gameObject.GetComponent<ReactiveObject>();
        return null;
    }

    //Print a gui aim
    private void OnGUI()
    {
        int size = 24;
        float posX = _camera.pixelWidth / 2 - size / 4;
        float posY = _camera.pixelHeight / 2 - size / 2;

        GUI.Label(new Rect(posX, posY, size, size), "[ o ]", style);

        //For testing purposes
        GUIStyle style2 = new GUIStyle();
        style2.fontSize = 22;
        style2.normal.textColor = Color.white;
        size = 380;
        posX = 100;
        posY = 600;
        GUI.Label(new Rect(posX, posY, size, size), "Attracting= " + attracting + "\nReleasing= " + releasing + "\nLaunching= " + launching + "\nIncreasing= " + increasing + "\nDecreasing= " + decreasing + "\nShooting= " + shooting, style2);
    }
}
