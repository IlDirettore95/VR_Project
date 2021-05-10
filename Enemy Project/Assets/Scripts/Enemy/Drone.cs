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
        Stunned,
        Dead
    }

    //Attack
    public float attackDamage;
    public float fireCooldown;
    private float nextTimeFire;
    public Transform firePoint;
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

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.Find("Player").transform;
        _playerStatus = playerTransform.GetComponent<PlayerStatus>();
        fs = GetComponent<FireStatus>();

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

        if (!isAlive) _currentState = DroneState.Dead;

        switch (_currentState)  //Finite State Machine
        {
            case DroneState.Dead:
                {
                    Destroy(gameObject);

                    break;
                }
            case DroneState.Guarding:
                {
                    if (playerDistance <= triggerPlayerDistance)
                    {
                        _droneBody.transform.LookAt(playerTransform);
                        nextTimeFire = Time.time + fireCooldown;
                        lastPlayerPosition = playerTransform.position;
                        TriggerArea(areaID);

                        _currentState = DroneState.Chasing;
                    }

                    break;
                }
            case DroneState.Patrolling:
                {
                    if (playerDistance <= triggerPlayerDistance)
                    {
                        _droneBody.transform.LookAt(playerTransform);
                        nextTimeFire = Time.time + fireCooldown;
                        lastPlayerPosition = playerTransform.position;
                        TriggerArea(areaID);

                        _currentState = DroneState.Chasing;
                    }

                    break;
                }
            case DroneState.Chasing:
                {
                    _droneBody.transform.LookAt(playerTransform);

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
                    }
                    else if(!_agent.isOnOffMeshLink)
                    {
                        Vector3 direzione = -(transform.position - playerTransform.position).normalized;

                        RaycastHit hit;
                        if (Physics.SphereCast(transform.position, 0.3f, direzione, out hit, 50f))
                        {
                            GameObject hitObject = hit.transform.gameObject;
                            if (hitObject.tag.Equals("Player")) // || (hitObject.GetComponent<ReactiveObject>() != null && hitObject.GetComponent<ReactiveObject>().IsAttracted())
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

                    if (playerDistance <= attackDistance)
                    {
                        if (Time.time >= nextTimeFire)
                        {
                            Shoot();
                            nextTimeFire = Time.time + fireCooldown;
                        }
                    }
                    else
                    {
                        _agent.isStopped = false;
                        _currentState = DroneState.Chasing;
                    }
                        
                    break;
                }
            case DroneState.Attracted:
                {
                    RaycastHit hit;
                    if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit))
                    {
                        if (hit.transform.gameObject.tag.Equals("Player") && Time.time >= nextTimeFire)
                        {
                            Shoot();
                            nextTimeFire = Time.time + fireCooldown;
                        }
                    }

                    break;
                }
            case DroneState.Throwed:
                {
                    speed = rb.velocity.magnitude;

                    if (speed < 3)
                    {
                        rb.useGravity = false;
                        rb.velocity = Vector3.zero;

                        currentStateBuildUp = 0f;
                        navmeshFinded = false;
                        _currentState = DroneState.Stunned;
                    }

                    break;
                }
            case DroneState.Stunned:
                {
                    //_droneBody.transform.LookAt(playerTransform);
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
        Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
    }

    //Handles collision damage
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody colliderRb = collision.gameObject.GetComponent<Rigidbody>();
        if (colliderRb != null)
        {
            if (collision.relativeVelocity.magnitude > impactVelocityThreashold)
            {
                if (_currentState == DroneState.Patrolling || _currentState == DroneState.Guarding)
                {
                    TriggerArea(areaID);
                }

                if (_currentState != DroneState.Throwed)
                {
                    _agent.enabled = false;
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.AddForce(collision.relativeVelocity, ForceMode.Impulse);
                    _currentState = DroneState.Throwed;
                }

                Hurt(colliderRb.mass * (collision.relativeVelocity.magnitude - impactVelocityThreashold));
            }
        }
        //The collided object is assumed to be static
        else if (speed > impactVelocityThreashold)
        {
            Hurt((speed - impactVelocityThreashold));
        }
    }

    public override void ReactToAttraction(float attractionSpeed)
    {
        base.ReactToAttraction(attractionSpeed);

        _currentState = DroneState.Attracted;
    }

    public override void ReactToReleasing()
    {
        base.ReactToReleasing();

        _currentState = DroneState.Throwed;
    }

    public override void ReactToLaunching(float launchingSpeed)
    {
        base.ReactToLaunching(launchingSpeed);

        _currentState = DroneState.Throwed;
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
                startPos = _droneBody.transform.position; Debug.Log("StartPos: " + startPos);
                targetPos = hit.point + Vector3.up * _agent.baseOffset;
                pos = 0;
                rb.isKinematic = true;
                if(startPos != targetPos) navmeshFinded = true;
            }
        }
    }

    public override void Triggered()
    {
        if(_currentState == DroneState.Patrolling || _currentState == DroneState.Guarding)
        {
            _currentState = DroneState.Chasing;
        }
    }
}
