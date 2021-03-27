using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Death))]
[AddComponentMenu("Control Script/FPS Input with Jump and Jetpack")]

public class FPSInput_Jump_Jetpack : MonoBehaviour
{
    public float gravity = -9.8f;
    public float gravityMultiplier = 2f;
    public float jumpSpeed = 9.0f;
    public float jetPackY = 7.0f;
    public float runningSpeed = 12f;
    public float walkingSpeed = 6f;
    public float flyingSpeed = 6f;
    public int maxFuel = 120;
    public const int minFuel = 0;
    public int dischargingRate = 60;
    public int dischargingActivation = 20;
    public int chargingRate = 30;
    public float fallDamageThreashold;

    private bool jetpack = false;
    private bool canCharge = true;
    private bool isFalling = false;
    private float fuel;
    private float startOfFall;
    private bool hasFallen = false;
    

    private CharacterController _charController;
    private Death _deathController;
    private float _deltaY = 0f; //Remember your Y velocity e.g. during a jump

    // Start is called before the first frame update
    void Start()
    {
        fuel = maxFuel;
        _charController = GetComponent<CharacterController>();
        _deathController = GetComponent<Death>();
    }

    // Update is called once per frame
    void Update()
    {
        //The player's jetpack will start charging whenever he is grounded and it will continue until he reactivates the jetpack
        if (canCharge)
        {
            if (fuel < maxFuel)
            {
                fuel += chargingRate * Time.deltaTime;
                if (fuel > maxFuel) { fuel = maxFuel; }
            }
        }
        Debug.Log("fuel: " +fuel);
        float deltaX = Input.GetAxis("Horizontal");
        float deltaZ = Input.GetAxis("Vertical");
        float speed = walkingSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && !isFalling)
        {
            speed = runningSpeed;

        }

        //Vertical movement (jumping)
        if (_charController.isGrounded )
        {
            if (hasFallen && (startOfFall - transform.position.y) > fallDamageThreashold)
            {
                _deathController.Hurt((startOfFall - transform.position.y - fallDamageThreashold));
            }
            canCharge = true;
            isFalling = false;
            hasFallen = false;
            if (Input.GetButtonDown("Jump"))
            {
                _deltaY = jumpSpeed;  
            }
        }
        else
        {
            if(!hasFallen)
            {
                hasFallen = true;
                startOfFall = transform.position.y;
            }
            if (Input.GetButtonDown("Jump") && !jetpack && fuel > 0)
            {
                fuel -= dischargingActivation;
                if (fuel < minFuel) fuel = minFuel;
                if (fuel == minFuel) jetpack = false;
                _deltaY = jetPackY;
                jetpack = true;
                canCharge = false;
            }
            else if (Input.GetButton("Jump") && jetpack)
            {
                fuel -= dischargingRate * Time.deltaTime;
                if (fuel < minFuel) fuel = minFuel;
                if (fuel == minFuel)
                {
                    jetpack = false;
                    isFalling = true;
                }
                _deltaY = jetPackY;
                speed = flyingSpeed;
            }
            else if(Input.GetButtonUp("Jump") && jetpack)
            {
                jetpack = false;
                isFalling = true;
            }
            
            _deltaY += gravity * gravityMultiplier * Time.deltaTime;
            
        }
        deltaX *= speed;
        deltaZ *= speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = _deltaY;
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
    }

    public void resetY()
    {
        _deltaY = 0;
    }

}
