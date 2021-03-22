using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input with Jump and Jetpack")]

public class FPSInput_Jump_Jetpack : MonoBehaviour
{
    private float speed = 0f;
    public float gravity = -9.8f;
    public float gravityMultiplier = 2f;
    public float jumpSpeed = 9.0f;
    private float fuel;
    public int maxFuel = 100;
    public const int minFuel = 0;
    public int dischargingRate = 50;
    public int dischargingActivation = 10;
    public int chargingRate = 10;
    private bool jetpack = false;
    public float runningSpeed = 12f;
    public float walkingSpeed = 6f;
    public float flyingSpeed = 6f;

    private CharacterController _charController;
    private float _deltaY = 0f; //Remember your Y velocity e.g. during a jump

    // Start is called before the first frame update
    void Start()
    {
        fuel = maxFuel;
        _charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
       
        Debug.Log("fuel: " +fuel);
        
        float deltaX = Input.GetAxis("Horizontal") ;
        float deltaZ = Input.GetAxis("Vertical") ;
        //Vertical movement (jumping)
        if (_charController.isGrounded )
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = runningSpeed;
                
            }
            else
            {
                speed = walkingSpeed;
            }
            
            deltaX *= speed;
            deltaZ *= speed;
            
            if (fuel < maxFuel)
            {
                fuel += chargingRate * Time.deltaTime;
                if (fuel > maxFuel){ fuel = maxFuel;}
            }
            

            if (Input.GetButtonDown("Jump"))
            {
                _deltaY = jumpSpeed;
                
            }
        }
        
        else
        {
            speed = flyingSpeed;
            deltaX *= speed;
            deltaZ *= speed;
            
            if (Input.GetButtonDown("Jump") && !jetpack && fuel > 0)
            {
                fuel -= dischargingActivation;
                if (fuel < minFuel) fuel = minFuel;
                if (fuel == minFuel) jetpack = false;
                _deltaY = jumpSpeed;
                jetpack = true;
            }
            else if (Input.GetButton("Jump") && jetpack)
            {
                fuel -= dischargingRate * Time.deltaTime;
                if (fuel < minFuel) fuel = minFuel;
                if (fuel == minFuel) jetpack = false;
                _deltaY = jumpSpeed;
            }
            else if(Input.GetButtonUp("Jump"))
            {
                jetpack = false;
            }
            
            _deltaY += gravity * gravityMultiplier * Time.deltaTime;
            
        }
       
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
