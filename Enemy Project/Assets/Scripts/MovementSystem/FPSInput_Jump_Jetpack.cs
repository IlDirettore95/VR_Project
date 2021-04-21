using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStatus))]
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

    public float dischargingRate = 60;
    public float dischargingActivation = 20;
    public float chargingRate = 30;
    public float consumingRate = 20;
    public float recoveringRate = 35;
    public float recoverCooldown = 4;
    private float nextTimeRecover;
    public float fallDamageThreashold;

    private bool jetpack = false;
    private bool canCharge = true;
    private bool isFalling = false;
    private float fuel;
    private float startOfFall;
    private bool hasFallen = false;
    private bool isJumping = false;
    private bool running = false;
    

    private CharacterController _charController;
    private PlayerStatus _status;
    private float _deltaY = 0f; //Remember your Y velocity e.g. during a jump

    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _status = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        //The player's jetpack will start charging whenever he is grounded and it will continue until he reactivates the jetpack
        if (canCharge && !_status.IsFullFuel())
        {
            _status.RecoverFuel(chargingRate * Time.deltaTime);
        }
        float deltaX = Input.GetAxis("Horizontal");
        float deltaZ = Input.GetAxis("Vertical");
        float speed = walkingSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && !isFalling && _status.HasEnoughEnergy())
        {
            running = true;
            speed = runningSpeed;
            if(_charController.isGrounded) _status.ConsumeStamina(consumingRate * Time.deltaTime);
            if (!_status.HasEnoughEnergy()) nextTimeRecover = Time.time + recoverCooldown;
        }
        else
        {
            running = false;
        }
        //Vertical movement (jumping)
        if (_charController.isGrounded)
        {
            if(!running && Time.time > nextTimeRecover)
            {
                _status.RecoverStamina(recoveringRate * Time.deltaTime);
            }
            if (hasFallen && (startOfFall - transform.position.y) > fallDamageThreashold)
            {
                _status.Hurt((startOfFall - transform.position.y - fallDamageThreashold));
            }
            canCharge = true;
            isFalling = false;
            hasFallen = false;
            isJumping = false;
            if (Input.GetButtonDown("Jump"))
            {
                _deltaY = jumpSpeed;
                isJumping = true;
            }
        }
        else
        {
            if(!hasFallen && !jetpack)
            {
                hasFallen = true;
                startOfFall = transform.position.y;
                if(!isJumping && !isFalling) _deltaY = 0;
            }
            if (Input.GetButtonDown("Jump") && !jetpack && _status.HasEnoughFuel())
            {
                _status.ConsumeFuel(dischargingActivation);
                _deltaY = jetPackY;
                speed = flyingSpeed;
                canCharge = false;
                if (_status.HasEnoughFuel()) jetpack = true;
                else
                {
                    jetpack = false;
                    isFalling = true;
                    hasFallen = false;
                }
            }
            else if (Input.GetButton("Jump") && jetpack)
            {
                _status.ConsumeFuel(dischargingRate * Time.deltaTime);
                _deltaY = jetPackY;
                speed = flyingSpeed;
                if (!_status.HasEnoughFuel())
                {
                    jetpack = false;
                    isFalling = true;
                    hasFallen = false;
                }
                
            }
            else if(Input.GetButtonUp("Jump") && jetpack)
            {
                jetpack = false;
                isFalling = true;
                hasFallen = false;
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
