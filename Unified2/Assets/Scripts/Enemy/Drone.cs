using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Drone : Enemy
{
    //Enemy state
    public enum DroneState
    {
        Guarding,
        Patrolling,
        Chasing,
        Attack,
        Throwed,
        Attracted,
        Recovery,
        Dead
    }

    //Attack
    public float attackDamage;
    public float fireCooldown;
    public float fireRate;
    private float nextTimeFire;
    private float nextTimeCooldown;
    //private bool coolDown = false;
    public Transform[] firePoints;
    public GameObject projectilePrefab;

    //AI
    private Vector3 lastPlayerPosition;
    private Vector3 startPos;
    private Vector3 targetPos;
    private readonly float posBuildUp = 1f;
    private float pos;
    private DroneState _currentState;
    public GameObject _droneBody;
    private bool navmeshFinded = false;
    private bool stunned = false;

    //Rotation of sight before moving
    public float sightRotationThreashold;
    public float rotationSpeed;

    //VFX
    [SerializeField] Light _internalLight;
    [SerializeField] Light _externalLight;
    private LightFlickering lfInt;
    private LightFlickering lfExt;

    //Animation
    Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.Find("Player").transform;
        _playerStatus = playerTransform.GetComponent<PlayerStatus>();
        fs = GetComponent<FireStatus>();
        _animator = GetComponentInChildren<Animator>();
        lfInt = GetComponents<LightFlickering>()[0];
        lfExt = GetComponents<LightFlickering>()[1];

        rb.useGravity = false;
        rb.isKinematic = true;
        _health = MaxHealth;
        target = GameObject.Find("ObjectGrabber").transform;

        _currentState = DroneState.Guarding;
    }

    // Update is called once per frame
    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);

        if (!isAlive)
        {
            _currentState = DroneState.Dead;
            _internalLight.enabled = false;
            _externalLight.enabled = false;
            lfInt.enabled = false;
            lfExt.enabled = false;
        }

        switch (_currentState)  //Finite State Machine
        {
            case DroneState.Dead:
                {
                    break;
                }
            case DroneState.Guarding:
                {
                    if (patrollingPoints.Length >= 2 && Time.time >= nextTimePatrol)
                    {
                        ChangeDestination();
                        Patrol();

                        _currentState = DroneState.Patrolling;
                    }

                    findPlayer();

                    /*
                    if (playerDistance <= triggerPlayerDistance)
                    {
                        //_droneBody.transform.LookAt(playerTransform);
                        nextTimeFire = Time.time + fireCooldown;
                        lastPlayerPosition = playerTransform.position;
                        TriggerArea(areaID);
                        _animator.SetBool("Triggered", true);

                        _internalLight.color = new Color(1, 0, 0, 1);
                        _externalLight.color = new Color(1, 0, 0, 1);

                        _currentState = DroneState.Chasing;
                    }
                    */

                    break;
                }
            case DroneState.Patrolling:
                {
                    if (isAtDestination())
                    {
                        nextTimePatrol = Time.time + guardingCooldown;
                        _currentState = DroneState.Guarding;
                    }

                    findPlayer();

                    /*
                    if (playerDistance <= triggerPlayerDistance)
                    {
                        //_droneBody.transform.LookAt(playerTransform);
                        nextTimeFire = Time.time + fireCooldown;
                        lastPlayerPosition = playerTransform.position;
                        TriggerArea(areaID);
                        _animator.SetBool("Triggered", true);

                        _internalLight.color = new Color(1, 0, 0, 1);
                        _externalLight.color = new Color(1, 0, 0, 1);

                        _currentState = DroneState.Chasing;
                    }
                    */

                    break;
                }
            case DroneState.Chasing:
                {
                    _droneBody.transform.rotation = transform.rotation;

                    
                    //Lerping speed to triggered speed
                    currentStateBuildUp += triggeredSpeedBuildUp * Time.deltaTime;
                    speed = Mathf.Lerp(walkingSpeed, triggeredSpeed, currentStateBuildUp);
                    _agent.speed = speed;
                    

                    if (playerDistance > attackDistance)
                    {
                        
                        _agent.SetDestination(playerTransform.position);

                        if (_agent.hasPath)
                        {
                            lastPlayerPosition = _agent.destination;
                        }
                        else
                        {
                            _agent.SetDestination(lastPlayerPosition);
                        }
                        

                        /*
                        //Following the player
                        //Calculate the angle beetween the enemy's transform.forward and the player
                        float angle = Vector3.SignedAngle((playerTransform.position - transform.position), transform.forward, Vector3.up);
                        if (angle < -sightRotationThreashold || angle > sightRotationThreashold)
                        {
                            //turn left or turn right
                            _agent.isStopped = true;
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((playerTransform.position - transform.position).normalized), rotationSpeed * Time.deltaTime);
                        }
                        else
                        {
                            _agent.isStopped = false;

                            //Lerping speed to triggered speed
                            currentStateBuildUp += triggeredSpeedBuildUp * Time.deltaTime;
                            speed = Mathf.Lerp(walkingSpeed, triggeredSpeed, currentStateBuildUp);
                            _agent.speed = speed;

                            //Finding the player
                            _agent.SetDestination(playerTransform.position);

                            if (_agent.hasPath)
                            {
                                lastPlayerPosition = _agent.destination;
                            }
                            else
                            {
                                _agent.SetDestination(lastPlayerPosition);
                            }
                        }
                        */
                    }
                    else if(!_agent.isOnOffMeshLink)
                    {
                        Vector3 direzione = -(transform.position - playerTransform.position).normalized;

                        RaycastHit hit;
                        if (Physics.SphereCast(transform.position, 0.3f, direzione, out hit, 50f))
                        {
                            GameObject hitObject = hit.transform.gameObject;
                            if (hitObject.tag.Equals("Player"))
                            {
                                _agent.isStopped = true;
                                _droneBody.transform.LookAt(playerTransform);
                                _currentState = DroneState.Attack;
                            }
                        }
                    }
                    
                    break;
                }
            case DroneState.Attack:
                {
                    _droneBody.transform.LookAt(playerTransform);

                    float angle = Vector3.SignedAngle((playerTransform.position - transform.position), transform.forward, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((playerTransform.position - transform.position).normalized), 1);

                    if (playerDistance <= attackDistance)
                    {
                        if (Time.time >= nextTimeFire)
                        {
                            InvokeRepeating("Shoot", 0f, 1f / fireRate);
                            nextTimeCooldown = Time.time + fireCooldown;
                            nextTimeFire = Time.time + 2 * fireCooldown;
                        }
                        else if (Time.time >= nextTimeCooldown)
                        {
                            CancelInvoke("Shoot");
                        }
                    }
                    else
                    {
                        CancelInvoke("Shoot");
                        _agent.isStopped = false;
                        _currentState = DroneState.Chasing;
                    }
                        
                    break;
                }
            case DroneState.Attracted:
                {
                    /*
                    RaycastHit hit;
                    if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit))
                    {
                        if (hit.transform.gameObject.tag.Equals("Player") && Time.time >= nextTimeFire)
                        {
                            Shoot();
                            nextTimeFire = Time.time + fireCooldown;
                        }
                    }
                    */

                    break;
                }
            case DroneState.Throwed:
                {
                    speed = rb.velocity.magnitude;

                    if (speed < 3 && stunned)
                    {
                        stunned = false;
                        StartCoroutine(StartRecovery());
                    }

                    break;
                }
            case DroneState.Recovery:
                {
                    _droneBody.transform.LookAt(playerTransform);
                    transform.LookAt(playerTransform);

                    if (!navmeshFinded)
                    {
                        Scan(transform.forward);
                        Scan(-transform.forward);
                        Scan(transform.up);
                        Scan(-transform.up);
                        Scan(transform.right);
                        Scan(-transform.right);

                        FindNavMesh();
                    }
                    else if (_droneBody.transform.position.Equals(targetPos))
                    {
                        _agent.enabled = true;
                        navmeshFinded = false;
                        lfInt.enabled = false;
                        lfExt.enabled = false;

                        _currentState = DroneState.Chasing;
                    }
                    else
                    {
                        pos += posBuildUp * Time.deltaTime;
                        transform.position = Vector3.Lerp(startPos, targetPos, pos);
                        _agent.transform.position = Vector3.Lerp(startPos, targetPos, pos);
                        //_droneBody.transform.position = Vector3.Lerp(startPos, targetPos, pos);
                    }

                    break;
                }
        }
    }

    private void Shoot()
    {
        System.Random rnd = new System.Random();
        int rand = rnd.Next(2);
        GameObject projectile = Instantiate(projectilePrefab, firePoints[rand].transform.position, firePoints[rand].transform.rotation);
        projectile.transform.LookAt(playerTransform);
    }

    //Handles collision damage
    private void OnCollisionEnter(Collision collision)
    {
        if (isAlive)
        {
            Rigidbody colliderRb = collision.gameObject.GetComponent<Rigidbody>();
            if (colliderRb != null)
            {
                if (collision.relativeVelocity.magnitude > impactVelocityThreashold)
                {
                    if (_currentState == DroneState.Patrolling || _currentState == DroneState.Guarding)
                    {
                        TriggerArea(areaID);

                        _internalLight.color = new Color(1, 0, 0, 1);
                        _externalLight.color = new Color(1, 0, 0, 1);
                    }

                    if (_currentState != DroneState.Throwed)
                    {
                        _agent.enabled = false;
                        rb.isKinematic = false;
                        rb.useGravity = true;
                        rb.AddForce(collision.relativeVelocity, ForceMode.Force);
                        stunned = true;

                        if (_currentState == DroneState.Attack) CancelInvoke("Shoot");

                        _currentState = DroneState.Throwed;
                    }

                    Hurt(colliderRb.mass * (collision.relativeVelocity.magnitude - impactVelocityThreashold));
                }
            }
            //The collided object is assumed to be static
            else if (_currentState == DroneState.Throwed)
            {
                stunned = true;

                if (speed > impactVelocityThreashold)
                    Hurt((speed - impactVelocityThreashold));
            }
        }
    }

    public override void ReactToAttraction(float attractionSpeed)
    {
        base.ReactToAttraction(attractionSpeed);

        if (isAlive)
        {
            if (_currentState == DroneState.Attack) CancelInvoke("Shoot");

            _currentState = DroneState.Attracted;
        }
    }

    public override void ReactToReleasing()
    {
        base.ReactToReleasing();

        if (isAlive)
        {
            _currentState = DroneState.Throwed;
        }
    }

    public override void ReactToLaunching(float launchingSpeed)
    {
        base.ReactToLaunching(launchingSpeed);

        if (isAlive)
        {
            _currentState = DroneState.Throwed;
        }
    }

    private void Scan(Vector3 direction)
    {
        RaycastHit hit;
        Vector3 direzione;

        if (Physics.Raycast(transform.position, direction, out hit))
        {
            direzione = hit.point - transform.position;

            if (hit.distance < 4f)
            {
                rb.AddForce(-direzione.normalized * walkingSpeed);
            }
            else if (hit.distance < 2f)
            {
                rb.AddForce(-direzione.normalized * walkingSpeed * 2);
            }
        }
    }

    private void FindNavMesh()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            if (hit.transform.gameObject.GetComponent<NavMeshSurface>() != null)
            {
                startPos = _droneBody.transform.position;
                targetPos = hit.point + Vector3.up * _agent.baseOffset;
                pos = 0;
                rb.isKinematic = true;
                //if(startPos != targetPos) navmeshFinded = true;
                navmeshFinded = true;
                //if (navmeshFinded) Debug.Log("NavMesh finded!");
            }
        }
    }

    private void findPlayer()
    {
        Vector3 direzione = -(transform.position - playerTransform.position).normalized;

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.3f, direzione, out hit, triggerPlayerDistance))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.tag.Equals("Player"))
            {
                Triggered();
            }
        }
    }

    public override void Triggered()
    {
        if(_currentState == DroneState.Patrolling || _currentState == DroneState.Guarding)
        {
            nextTimeFire = Time.time + fireCooldown;
            lastPlayerPosition = playerTransform.position;
            StartCoroutine(TriggerArea(areaID));

            _animator.SetBool("Triggered", true);

            _internalLight.color = new Color(1, 0, 0, 1);
            _externalLight.color = new Color(1, 0, 0, 1);

            _currentState = DroneState.Chasing;
        }
    }

    private IEnumerator StartRecovery()
    {
        yield return new WaitForSeconds(2f);

        if(_currentState == DroneState.Throwed)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;

            currentStateBuildUp = 0f;
            navmeshFinded = false;

            lfInt.enabled = true;
            lfExt.enabled = true;

            _currentState = DroneState.Recovery;
        }
    }
}
