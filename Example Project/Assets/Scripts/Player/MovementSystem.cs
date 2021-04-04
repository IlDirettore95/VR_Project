using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    /*This script handles the movement system
     * The player could be in the idle, walking, running, falling status
     * This class also provides methods for questioning about player's movment status;
     * Idle: the player is not moving at all
     * Walking: the player is moving with WASD and grounded
     * Running: the player is moving with WASD and has pressed LeftShift
     * Crouching: the player crouch whith CTRL, in this status he can only walk, stand up or jumping
     * HasJumped: the player isn't grounded due to a jump
     * Tired: the player consumed all the stamina and need to re-click LeftShift in order to run when the stamina will be recovered
    */
    private bool idle = true;
    private bool walking = false;
    private bool running = false;
    private bool falling = false;
    private bool jetpack = false;
    private bool crouching = false;
    private bool hasJumped = false;
    private bool tired = false;

    //Y Speed variables
    public float gravity;
    private float ySpeed;
    public float initialGravity;
    public float jumpSpeed;
    public float jetpackYSpeed;
    //X and Z Speed variables
    public float walkingSpeed;
    public float runningSpeed;
    public float jetpackXZSpeed;
    public float fallingSpeed;
    //Buildups for lerping
    public float runningBuildUp;
    public float walkingBuildUp;
    public float fallingBuildUp;
    public float jetpackBuildUp;
    //X,Y,Z deltas
    private float deltaX = 0f;
    private float deltaY = 0f;
    public float minDeltaY; //Limit speed while falling
    private float deltaZ = 0f;
    private float speed = 0f; //Player's general speed

    //Fall Damage
    private float startFallingY = float.NegativeInfinity;
    public float fallDamageThreashold;

    //Stamina handling for running
    public float sRecoverRate;
    public float sConsumingRate;
    public float sCooldown; //Once consumed stamina will recover after a cooldown
    private float sNextTime; //The time on which stamina will start to recover

    //Fuel handling for jetpack
    public float fRecoverRate;
    public float fConsumigRate;
    public float fActivationCost;

    //Crouching Height
    public float normalHeight = 2f;
    public float crouchingHeight = 1f;
    public float crounchOnBuildUp = 3f;
    public float crounchOffBuildUp = 1f;

    private CharacterController _charController;
    private PlayerStatus _status;

    //Aux methods for external questioning on player movment status
    public bool IsIdle()
    {
        return idle;
    }

    public bool IsWalking()
    {
        return walking;
    }

    public bool IsRunning()
    {
        return running;
    }

    public bool IsFalling()
    {
        return falling;
    }

    public bool IsCrouching()
    {
        return crouching;
    }
    public bool IsUsingJetpack()
    {
        return jetpack;
    }

    public bool IsJumping()
    {
        return hasJumped;
    }

    public bool IsTired()
    {
        return tired;
    }
    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _status = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaX = Input.GetAxis("Horizontal");
        deltaZ = Input.GetAxis("Vertical");
        //Debug.Log("deltaY= " + deltaY + " gravity= " + ySpeed + " speed= " + speed + " t= " + Time.time);

        //Setting player status according to the listened inputs
        if (_charController.isGrounded)
        {

            //Fall damage
            if (falling)
            {
                float fallDistance = startFallingY - transform.position.y;
                if(fallDistance > fallDamageThreashold) _status.Hurt(fallDistance - fallDamageThreashold);
                startFallingY = float.NegativeInfinity;
            }

            //Stamina Recover, if the player is at full stamina is no more considered tired
            if (!running && Time.time > sNextTime) _status.RecoverStamina(sRecoverRate * Time.deltaTime);
            if (tired && _status.IsFullStamina()) tired = false;

            //Fuel Recover
            _status.RecoverFuel(fRecoverRate * Time.deltaTime);

            //Grounded
            falling = false;
            jetpack = false;
            hasJumped = false;
            ySpeed = initialGravity; //resetting Gravity

            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
                if(!crouching)
                {
                    crouching = true;
                    //Resize character controller
                    _charController.height = crouchingHeight;
                }
                else
                {
                    crouching = false;
                    _charController.height = normalHeight;
                }
            }

            //Is the player moving?
            if ((deltaX != 0 || deltaZ != 0))
            {
                if(Input.GetKey(KeyCode.LeftShift) && _status.HasEnoughEnergy() && !tired && !crouching)
                {
                    //running
                    running = true;
                    walking = false;
                    idle = false;

                    //Lerping from walk speed to running speed
                    speed = Mathf.Lerp(speed, runningSpeed, runningBuildUp * Time.deltaTime);

                    //Stamina consuming
                    _status.ConsumeStamina(sConsumingRate * Time.deltaTime);
                    if (!_status.HasEnoughEnergy())
                    {
                        sNextTime = Time.time + sCooldown;
                        tired = true;
                    }
                }
                else if(Input.GetKeyDown(KeyCode.LeftShift) && _status.HasEnoughEnergy() && tired)
                {
                    //If the player is tired he needs to re-click LeftShift in order to running or he could wait all the stamina is recoverd
                    //running
                    running = true;
                    walking = false;
                    idle = false;
                    tired = false;

                    //Lerping from walk speed to running speed
                    speed = Mathf.Lerp(speed, runningSpeed, runningBuildUp * Time.deltaTime);

                    //Stamina consuming
                    _status.ConsumeStamina(sConsumingRate * Time.deltaTime);
                    if (!_status.HasEnoughEnergy())
                    {
                        sNextTime = Time.time + sCooldown;
                        tired = true;
                    }
                }
                else
                {
                    //walking
                    walking = true;
                    running = false;
                    idle = false;

                    //Lerping from speed to running speed
                    speed = Mathf.Lerp(speed, walkingSpeed, walkingBuildUp * Time.deltaTime);
                }
                if (Input.GetButtonDown("Jump"))
                {
                    //The player is no more Grounded, he has jumped while walking or running
                    hasJumped = true;

                    //Reset Player height if he was crouching
                    crouching = false;
                    _charController.height = normalHeight;
                    deltaY = jumpSpeed;

                    //Lerping from speed to falling speed
                    speed = Mathf.Lerp(speed, fallingSpeed, fallingBuildUp * Time.deltaTime);
                }
            }
            else if(Input.GetButtonDown("Jump"))
            {
                //The player is no more Grounded, he has jumped while not moving
                hasJumped = true;
                idle = false;

                //Reset Player height if he was crouching
                crouching = false;
                _charController.height = normalHeight;

                deltaY = jumpSpeed;
            }
            else
            {
                idle = true;
                running = false;
                walking = false;

                //even when idle the speed will lerp to walking speed
                //Lerping from speed to walking speed
                speed = Mathf.Lerp(speed, walkingSpeed, walkingBuildUp * Time.deltaTime);
            }
        }
        else
        {
            //Erease accumulated gravity in case the player fall down walking or running
            if (!hasJumped)
            {
                deltaY = 0;
                hasJumped = true;
            }

            //NotGrounded
            walking = false;
            running = false;
            idle = false;

            if(Input.GetButtonDown("Jump") && _status.HasEnoughFuel())
            {
                //Jetpack Activation
                jetpack = true;
                falling = false;

                //Lerping from speed to falling speed
                speed = Mathf.Lerp(speed, jetpackXZSpeed, jetpackBuildUp * Time.deltaTime);

                deltaY = jetpackYSpeed;

                ySpeed = initialGravity; //resetting Gravity

                startFallingY = float.NegativeInfinity;

                _status.ConsumeFuel(fActivationCost);
                if (!_status.HasEnoughFuel())
                {
                    falling = true;
                    jetpack = false;
                }
            }
            else if(Input.GetButton("Jump") && jetpack && _status.HasEnoughFuel())
            {
                //Jetpack
                //Lerping from speed to falling speed
                speed = Mathf.Lerp(speed, jetpackXZSpeed, jetpackBuildUp * Time.deltaTime);
                deltaY = jetpackYSpeed;

                startFallingY = float.NegativeInfinity;

                _status.ConsumeFuel(fConsumigRate * Time.deltaTime);
                if (!_status.HasEnoughFuel())
                {
                    falling = true;
                    jetpack = false;
                }
            }
            else if(Input.GetButtonUp("Jump"))
            {
                //Jetpack disactivation
                falling = true;
                jetpack = false;

                //Lerping from speed to falling speed
                speed = Mathf.Lerp(speed, fallingSpeed, fallingBuildUp * Time.deltaTime);

                //Acceleration due to gravity and locking to limit speed
                ySpeed += gravity * Time.deltaTime;
                deltaY -= ySpeed * Time.deltaTime;
                deltaY = Mathf.Clamp(deltaY, minDeltaY, float.PositiveInfinity);

                startFallingY = transform.position.y;
            }
            else
            {
                //Falling
                falling = true;

                //Lerping from speed to falling speed
                speed = Mathf.Lerp(speed, fallingSpeed, fallingBuildUp * Time.deltaTime);

                //Acceleration due to gravity and locking to limit speed
                ySpeed += gravity * Time.deltaTime;
                deltaY -= ySpeed * Time.deltaTime;
                deltaY = Mathf.Clamp(deltaY, minDeltaY, float.PositiveInfinity);

                if (transform.position.y > startFallingY) startFallingY = transform.position.y;
            }
        }

        deltaX *= speed;
        deltaZ *= speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = deltaY;
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
    }

    private void OnGUI()
    {
        //For testing purposes
        GUIStyle style = new GUIStyle();
        style.fontSize = 22;
        style.normal.textColor = Color.white;
        int size = 380;
        float posX = 1700;
        float posY = 600;
        GUI.Label(new Rect(posX, posY, size, size), "Idle= " + idle + "\nWalking= " + walking + "\nRunning= " + running + "\nCrouching= " + crouching + "\nFalling= " + falling + "\nJetpack= " + jetpack + "\nHasJumped= " + hasJumped + "\nTired= " + tired + "\nMaxY= " + startFallingY, style);
    }

    public void reset()
    {
        deltaX = 0;
        deltaZ = 0;
        deltaY = 0;

        idle = true;
        walking = false;
        running = false;
        falling = false;
        jetpack = false;

    }
}
