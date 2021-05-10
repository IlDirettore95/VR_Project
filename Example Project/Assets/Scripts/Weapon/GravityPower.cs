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
    public KeyCode releasingKey;
    public KeyCode launchingKey;
    public KeyCode repulsingKey;
    public KeyCode increaseKey;
    public KeyCode decreaseKey;
    public KeyCode shootingKey;

    //GravityPowers status
    private bool attracting = false;
    private bool releasing = false;
    private bool launching = false;
    private bool repulsing = false;
    private bool increasing = false;
    private bool decreasing = false;
    private bool shooting = false;

    //Aux methods for external questioning
    public bool IsAttracting() => attracting;
    public bool IsReleasing() => releasing;
    public bool IsLaunching() => launching;
    public bool IsRepulsing() => repulsing;
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
        if (Input.GetKeyDown(attractionKey) && !attracting && !shooting) ActiveAttraction();

        else if (Input.GetKeyUp(releasingKey) && attracting) Releasing();

        else if (Input.GetKeyDown(launchingKey) && attracting) Launching();

        else if (Input.GetKeyDown(repulsingKey) && !attracting && !shooting) Repulsing();

        else if (Input.GetKeyDown(increaseKey) && !attracting && !shooting) Increasing();

        else if (Input.GetKeyDown(decreaseKey) && !attracting && !shooting) Decreasing();

        else if (Input.GetKeyDown(shootingKey) && !attracting && !shooting) Shooting();

    }

    private void ActiveAttraction()
    {
        attracting = true;
    }

    private void Releasing()
    {
        attracting = false;
    }

    private void Launching()
    {
        attracting = false;
    }

    private void Repulsing()
    {

    }

    private void Increasing()
    {

    }

    private void Decreasing()
    {

    }

    private void Shooting()
    {

    }

    //Print a gui aim
    private void OnGUI()
    {
        int size = 24;
        float posX = _camera.pixelWidth / 2 - size / 4;
        float posY = _camera.pixelHeight / 2 - size / 2;

        GUI.Label(new Rect(posX, posY, size, size), "[ o ]", style);
    }
}
