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
      * Crouched: the player crouch whith C, in this status he can only walk, stand up or jumping
      * Crouching: the player is transitioning from crouched/stand to stand/crouched state
      * HasJumped: the player isn't grounded due to a jump
      * Tired: the player consumed all the stamina and need to re-click LeftShift in order to run when the stamina will be recovered
      * WasGrounded: the player was grounded during last frame. This state is used to capture in what frame the player changes from grounded to notgrounded
     */
    [HideInInspector] public bool idle = false;
    [HideInInspector] public bool walking = false;
    [HideInInspector] public bool running = false;
    [HideInInspector] public bool falling = false;
    [HideInInspector] public bool jetpack = false;
    [HideInInspector] public bool crouching = false;
    [HideInInspector] public bool crouched = false;
    [HideInInspector] public bool hasJumped = false;
    [HideInInspector] public bool tired = false;
    [HideInInspector] public bool wasGrounded = false;
    [HideInInspector] public bool isGrounded = false;

    //Isgrounded Threashold
    public float isGroundedThreashold;

    //Y Speed variables
    public float gravity;
    private float ySpeed; //Accumulated gravity
    public float initialGravity;
    public float jumpSpeed;

    //X and Z Speed variables
    public float walkingSpeed;
    public float runningSpeed;
    public float crouchingSpeed;
    public float fallingSpeed;

    //Buildups for lerping
    public float runningBuildUp;
    public float walkingBuildUp;
    public float fallingBuildUp;
    public float crouchOnBuildUp; //crouching transition on
    public float crouchOffBuildUp; //crouching transition off
    public float crouchingBuildUp; //X,Z speed
    private float currentBuildUp; //Player's current build up speed
    private float currentStateBuildUp; //BuildUp's current state, everytime a speed is changed is reset to 0

    //X,Y,Z deltas
    private float deltaX = 0f;
    private float deltaY = 0f;
    public float minDeltaY; //Limit speed while falling
    private float deltaZ = 0f;
    private float speed = 0f; //Player's general speed
    private float currentTargetSpeed = 0f;
    private float oldTargetSpeed = 0f; 

    //Fall Damage
    [HideInInspector] public float startFallingY = float.NegativeInfinity;
    public float fallDamageThreashold;

    //Stamina handling for running
    [Range(0, 1)] public float tiredThreashold;
    public float staminaConsumingRate; //for running
    public float staminaJumpCost; //for jumping

    //Crouching Height
    public float normalHeight;
    public float crouchingHeight;
    private float crouchingStateBuildUp; //This variable has to be resetted whenever crouching state varies 

    //Grouded state
    public float checkGroudedRadius;

    //Sliding on edges
    public float slidingFactor;

    private CharacterController _charController;
    private PlayerStatus _playerStatus;
    private StaminaRecover _staminaRecover;

    //Animation
    PlayerAnimationController _animController;


    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _playerStatus = GetComponent<PlayerStatus>();
        _staminaRecover = GetComponent<StaminaRecover>();
        _animController = GetComponent<PlayerAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaX = Input.GetAxis("Horizontal");
        deltaZ = Input.GetAxis("Vertical");

        //Setting player status according to the listened inputs
        SetIsGrounded();

        //Check for edges
        if(!isGrounded) SlidingEdges();

        if (isGrounded)
        {

            //Fall damage and setting the grounded state
            if (!wasGrounded)
            {
                if (falling) HandlingFallDamage();

                SettingGroundedState();
            }

            //Crouching status
            if (Input.GetKeyDown(KeyCode.C) && !crouching) SettingCrouchStatus();

            //Is the player moving?
            if ((deltaX != 0 || deltaZ != 0))
            {
                //DeltaZ != 1 means that the player cant run backward
                if (Input.GetKey(KeyCode.LeftShift) && !tired && !crouching && !crouched && deltaZ != -1) Run();

                //The player is surely walking
                else Walk();

                if (CanJump() && Input.GetButtonDown("Jump")) Jump();
            }
            //Jumping
            else if (CanJump() && Input.GetButtonDown("Jump")) Jump();

            else
            {
                Idle();
            }
        }
        //NotGrounded
        else
        {
            if (wasGrounded)
            {
                SettingNotGroundedState();
                //turn off stamina recovering while not grounded
                if (_staminaRecover.enabled == true) _staminaRecover.enabled = false;
            }
        }

        if(crouching) Crouch();

        //Stamina Recover, if the player is at full stamina is no more considered tired
        StaminaRecover();
    }

    //Do to script dependecies to jetpack script, falling status and character move are moved in the late Update
    void LateUpdate()
    {
        if (!isGrounded && !jetpack) Falling();
        
        //last controls on was grounded boolean
        if (isGrounded)
        {
            if (!wasGrounded) wasGrounded = true;
        }
        else
        {
            if (wasGrounded) wasGrounded = false;
        }
        
        HandlingSpeed();
        Move();
    }

    void OnGUI()
    {
        //For testing purposes
        GUIStyle style = new GUIStyle();
        style.fontSize = 22;
        style.normal.textColor = Color.white;
        int size = 380;
        float posX = 1700;
        float posY = 600;
        GUI.Label(new Rect(posX, posY, size, size), "Idle= " + idle + "\nWalking= " + walking + "\nRunning= " + running + "\nCrouching= " + crouching + "\nCrouched= " + crouched + "\nFalling= " + falling + "\nJetpack= " + jetpack + "\nHasJumped= " + hasJumped + "\nTired= " + tired + "\nMaxY= " + startFallingY + "\nIsGrounded= " + isGrounded + "\nWasGrounded= " + wasGrounded, style);
    }

    /* Handling fall damage
     *
    */
    private void HandlingFallDamage()
    {
        float fallDistance = startFallingY - transform.position.y;
        if (fallDistance > fallDamageThreashold) _playerStatus.Hurt(fallDistance - fallDamageThreashold);
        startFallingY = float.NegativeInfinity;
    }

    /*Setting the player's status when he becomes grounded
     */
    private void SettingGroundedState()
    {
        falling = false;
        hasJumped = false;
        ySpeed = initialGravity; //resetting Gravity
    }

    /* Setting the player's status when he becomes  not grounded
     */
    private void SettingNotGroundedState()
    {
        walking = false;
        running = false;
        idle = false;

        //Erease accumulated gravity in case the player fall down walking or running
        if (!hasJumped) deltaY = 0f;

        //Crouching off if the character was crouched or was trasitioning into crouched status
        if(crouched || crouching)
        {
            if(!crouched || !crouching) crouchingStateBuildUp = 1 - crouchingStateBuildUp;
            crouched = true;
            crouching = true;
        }  
    }

    /*Setting the crouch status when the player precc the crouch button
     */
    private void SettingCrouchStatus()
    {
        //Crouching state changes

        //If nothing is near above the player
        if (crouched && Physics.SphereCast(new Ray(transform.position, Vector3.up), _charController.radius, (normalHeight - crouchingHeight) / 2))
        {
            crouching = false;
        }
        else
        {
            crouching = true;
        }

        crouchingStateBuildUp = 0f;
    }

    /*Lerping for Crouch smoothness
     * This method adjust Player size and y position during crouching tranistion
    */
    private void Crouch()
    {
        float lastHeight = _charController.height;
        if (!crouched)
        {
            crouchingStateBuildUp += crouchOnBuildUp * Time.deltaTime;
            _charController.height = Mathf.Lerp(normalHeight, crouchingHeight, crouchingStateBuildUp);
            if (!hasJumped)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Ray(_charController.transform.position, Vector3.down), out hit))
                {
                    if (hit.distance <= (lastHeight / 2 + _charController.skinWidth + _charController.stepOffset)) _charController.Move(Vector3.down * hit.distance);
                }
            }
            if (_charController.height == crouchingHeight)
            {
                crouching = false;
                crouched = true;
            }
        }
        else
        {
            crouchingStateBuildUp += crouchOffBuildUp * Time.deltaTime;
            _charController.height = Mathf.Lerp(crouchingHeight, normalHeight, crouchingStateBuildUp);
            if (!hasJumped)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Ray(_charController.transform.position, Vector3.down), out hit))
                {
                    if (hit.distance <= lastHeight / 2 + _charController.skinWidth + _charController.stepOffset) _charController.Move(Vector3.up * (_charController.height / 2 + _charController.skinWidth - hit.distance));
                }
            }
            if (_charController.height == normalHeight)
            {
                crouching = false;
                crouched = false;
            }
        }
    }

    /*This method sets the player status to running and lerps the speed to running speed
     * Also handles stamina consuming during running
     */
    private void Run()
    {
        //running
        running = true;
        walking = false;
        idle = false;
        tired = false;

        //Lerping from walk speed to running speed
        SetSpeed(runningSpeed, runningBuildUp);

        //Stamina consuming
        _staminaRecover.enabled = false;
        _playerStatus.ConsumeStamina(staminaConsumingRate * Time.deltaTime);

        _animController.NextState();
    }

    /* Handling walking status and lerping speed tp walk speed 
     */
    private void Walk()
    {
        //walking
        walking = true;
        running = false;
        idle = false;

        if((!crouched && crouching) || (crouched && !crouching))
        {
            
            //Lerping from speed to crouching speed
            SetSpeed(crouchingSpeed, crouchingBuildUp);
        }
        else if((crouched && crouching) || (!crouched && !crouching))
        {
            //Lerping from speed to walking speed
            SetSpeed(walkingSpeed, walkingBuildUp);
        }

        _animController.NextState();
    }

    /* Handles jumping
     */
    private void Jump()
    {

        //The player is no more Grounded, he has jumped while walking or running
        hasJumped = true;
        idle = false;

        deltaY = jumpSpeed;

        ySpeed = initialGravity;

        //Lerping from speed to falling speed
        SetSpeed(fallingSpeed, fallingBuildUp);

        _animController.NextState();
    }

    /* Handles the idle state. In this state the speed will lerp to walk speed.
     */
    private void Idle()
    {
        //Player is idle
        idle = true;
        running = false;
        walking = false;

        //even when idle the speed will lerp to walking speed
        //Lerping from speed to walking speed
        SetSpeed(0f, 0f);

        _animController.NextState();
    }

    /*Handles the falling state. During the falling state the starting falling y will be updated to ensure a correct calculation of the fall damage
     */
    private void Falling()
    {
        //Falling
        falling = true;

        //Lerping from speed to falling speed
        SetSpeed(fallingSpeed, fallingBuildUp);

        //Earesing positive velocity on the vertical axis, in case of ceiling collision
        if ((_charController.collisionFlags & CollisionFlags.Above) != 0)
        {
            //The player has collided on a ceiling
            if (deltaY >= 0) deltaY = 0;
        }

        //Acceleration due to gravity and locking to limit speed
        ySpeed += gravity * Time.deltaTime;
        deltaY -= ySpeed * Time.deltaTime;
        deltaY = Mathf.Clamp(deltaY, minDeltaY, float.PositiveInfinity);

        

        //Updating strat falling position for handleing fall damage
        if (transform.position.y > startFallingY) startFallingY = transform.position.y;

        _animController.NextState();
    }


    /* This method will move the player according to deltaX, deltaZ and deltaY, ONLY if the player is not
     * in the idle state
    */
    private void Move()
    {
        deltaX *= speed;
        deltaZ *= speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = deltaY;
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
    }

    /*This method handles stamina recovering.
     * If the player is at 0 points of stamina he will be considered tired.
     * If he is tired and has more than tiredThreashold in % of stamina he won't be considered tired
     */
    private void StaminaRecover()
    {
        if (!running && !_playerStatus.IsFullStamina() && _staminaRecover.enabled == false) _staminaRecover.enabled = true;
        if (!_playerStatus.HasEnoughStamina()) tired = true;
        else if (tired && (_playerStatus.GetStamina() / _playerStatus.GetMaxStamina()) >= tiredThreashold) tired = false;
    }

    /* This script uses this method to establish if the character is grounded or not
     */
    public void SetIsGrounded()
    {
        RaycastHit hit;
        if (Physics.SphereCast(new Ray(_charController.transform.position, Vector3.down), _charController.radius * checkGroudedRadius, out hit))
        {
            isGrounded = hit.distance <= (_charController.height / 2 + _charController.skinWidth + _charController.stepOffset + isGroundedThreashold);
        }
        else
        {
            isGrounded = false;
        }
    }

    /* This method will set the new targeted speed and the new build up for lerping
     */
    public void SetSpeed(float newSpeed, float newBuildUp)
    {
        if (currentTargetSpeed != newSpeed)
        {
            oldTargetSpeed = speed;
            currentStateBuildUp = 0f;
        }
        currentTargetSpeed = newSpeed;
        currentBuildUp = newBuildUp;
        
    }

    /* This method will update player's speed frame per frame whatever it is using the currentBuildUp.
     * CurrentStateBuildUp is used update on the lerp
     */
    private void HandlingSpeed()
    {
        currentStateBuildUp += currentBuildUp * Time.deltaTime;
        speed = Mathf.Lerp(oldTargetSpeed, currentTargetSpeed, currentStateBuildUp);
    }

    /* This method is used to add a vertical speed to the movement.
     * Whenever this happens accumulated gravity will be deleted
     */
    public void SetYSpeed(float newSpeed)
    {
        deltaY = newSpeed;

        ySpeed = initialGravity; //resetting Gravity

        startFallingY = float.NegativeInfinity;
    }

    /* This method will slide the player away if he is too near the edges of a platform
     */
    private void SlidingEdges()
    {
        RaycastHit hit;
        Vector3 charPosition = _charController.transform.position;
        Vector3 origin = new Vector3(charPosition.x, charPosition.y - (_charController.height / 2 - _charController.radius), charPosition.z);
        if (Physics.SphereCast(new Ray(_charController.transform.position, Vector3.down), _charController.radius, out hit))
        {
            if (hit.distance <= (_charController.height / 2 + _charController.skinWidth + isGroundedThreashold))
            {
                Vector3 slidingMovement = transform.position - hit.point;
                slidingMovement.y = 0f;
                slidingMovement *= slidingFactor;
                _charController.Move(slidingMovement * Time.deltaTime);
            }
        }
    }

    /* If the character is crouched, he can only jump if there is enough space for he to stand
     */
    private bool CanJump()
    {
        if (crouched)
        {
            return !Physics.SphereCast(new Ray(transform.position, Vector3.up), _charController.radius, (normalHeight - crouchingHeight) / 2);
        }

        return true;
    }

    /* This power up will increase walking and running speed by a certain amount
     */
    public void PowerUpOn(float amount)
    {
        walkingSpeed += amount;
        runningSpeed += amount;
    }

    /* This power up will increase walking and running speed by a certain amount
     */
    public void PowerUpOff(float amount)
    {
        walkingSpeed -= amount;
        runningSpeed -= amount;
    }
}
