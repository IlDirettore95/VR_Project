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
    [SerializeField] private float attackDamage;
    [SerializeField] private float fireCooldown;
    [SerializeField] private float fireRate;
    private float nextTimeFire;
    private float nextTimeCooldown;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private GameObject projectilePrefab;

    //AI
    private Vector3 lastPlayerPosition;
    private Vector3 startPos;
    private Vector3 targetPos;
    private readonly float posBuildUp = 1f;
    private float pos;
    private DroneState _currentState;
    [SerializeField] private GameObject _droneBody;
    private bool navmeshFinded = false;
    private bool stunned = false;

    //VFX
    [SerializeField] Light _internalLight;
    [SerializeField] Light _externalLight;
    private LightFlickering lfInt;
    private LightFlickering lfExt;
    private AudioSource[] _audio;

    //Animation
    private Animator _animator;

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
        _audio = gameObject.GetComponents<AudioSource>();

        rb.useGravity = false;
        rb.isKinematic = true;
        _health = MaxHealth;
        target = GameObject.Find("ObjectGrabber").transform;

        isAlive = true;

        _currentState = DroneState.Guarding;
    }

    // Update is called once per frame
    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);

        if (!isAlive && _currentState != DroneState.Dead)
        {
            _currentState = DroneState.Dead;

            lfInt._enableLightOnDisable = false;
            lfExt._enableLightOnDisable = false;
            lfInt.enabled = false;
            lfExt.enabled = false;
            _internalLight.enabled = false;
            _externalLight.enabled = false;
            
            _audio[1].Play();
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

                    FindPlayer();

                    break;
                }
            case DroneState.Patrolling:
                {
                    if (isAtDestination())
                    {
                        nextTimePatrol = Time.time + guardingCooldown;
                        _currentState = DroneState.Guarding;
                    }

                    FindPlayer();

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
                            _agent.SetDestination(lastPlayerPosition); // If a path for the agent is not available, the enemy will go to the last known player position
                        }
                    }
                    else if(!_agent.isOnOffMeshLink) // If the drone is not on an offMeshLink, the player is inside his attack range and is visible, transit to the attack state
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
                    // Rotate the drone model towards the player
                    _droneBody.transform.LookAt(playerTransform);

                    // Rotate the navmesh agent towards the player. Is important for the navmesh agent that his orientation along the Y axis remains hortogonal to the surface. 
                    float angle = Vector3.SignedAngle((playerTransform.position - transform.position), transform.forward, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((playerTransform.position - transform.position).normalized), 1);

                    if (playerDistance <= attackDistance)
                    {
                        if (Time.time >= nextTimeFire) // The Time class was used to implement a cooldown before the next shoots burst
                        {
                            InvokeRepeating("Shoot", 0f, 1f / fireRate); // The InvokeRepeating handles the shoots burst
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
                        // After a suitable navmesh surface is finded, the current position is lerped to the correct position where the navmesh agent can be safely reactivated
                        pos += posBuildUp * Time.deltaTime;
                        transform.position = Vector3.Lerp(startPos, targetPos, pos);
                        _agent.transform.position = Vector3.Lerp(startPos, targetPos, pos);
                    }

                    break;
                }
        }
    }

    // Execute a single laser shoot
    private void Shoot()
    {
        System.Random rnd = new System.Random();
        int rand = rnd.Next(2);
        GameObject projectile = Instantiate(projectilePrefab, firePoints[rand].transform.position, firePoints[rand].transform.rotation);
        projectile.transform.LookAt(playerTransform);
        _audio[0].Play();
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
                {
                    Hurt((speed - impactVelocityThreashold));

                    _audio[2].Play();
                }
            }
        }
    }

    public override void ReactToAttraction(float attractionSpeed)
    {
        base.ReactToAttraction(attractionSpeed);

        if (isAlive)
        {
            if (_currentState == DroneState.Attack) CancelInvoke("Shoot");
            else if (_currentState == DroneState.Guarding || _currentState == DroneState.Patrolling) Triggered();

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

    // Used to avoid the surrounding obstacle while searching a navmesh surface in the recovery state
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

    // Try to find a suitable navmesh surface under the drone in order to reactive the NavMesh agent
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
                navmeshFinded = true; 
            }
        }
    }

    /* Before transit to the trigger state check if the player is currently visible for the enemy. 
     * This avoid that an enemy can transit on the chasing state when the player is nearby but in another room or hided behind a big object. */
    private void FindPlayer()
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

            // Changes the color of the drone's light from blue to red
            _internalLight.color = new Color(1, 0, 0, 1);
            _externalLight.color = new Color(1, 0, 0, 1);

            _currentState = DroneState.Chasing;
        }
    }

    // After 2 seconds of stunning, the drone start to recover his normal flying position
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

    public override bool IsDestroyed()
    {
        return false;
    }
}
