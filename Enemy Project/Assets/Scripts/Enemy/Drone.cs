using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Drone : Enemy
{
    //Enemy state
    private bool dead = false;
    public enum DroneState
    {
        Guard,
        Patrol,
        Chasing,
        Attack,
        Throwed,
        Attracted,
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

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.Find("Player").transform;
        _playerStatus = playerTransform.GetComponent<PlayerStatus>();

        rb.useGravity = false;
        rb.isKinematic = true;
        _health = MaxHealth;
        target = GameObject.Find("ObjectGrabber").transform;

        _currentState = DroneState.Guard;
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
            case DroneState.Guard:
                {
                    if (playerDistance <= triggerPlayerDistance)
                    {
                        _droneBody.transform.LookAt(playerTransform);

                        _currentState = DroneState.Chasing;
                        nextTimeFire = Time.time + fireCooldown;
                        lastPlayerPosition = playerTransform.position;
                    }

                    break;
                }
            case DroneState.Patrol:
                {
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
                    else
                    {
                        Vector3 direzione = -(transform.position - playerTransform.position).normalized;

                        RaycastHit hit;
                        if (Physics.SphereCast(transform.position, 0.3f, direzione, out hit, 50f))
                        {
                            GameObject hitObject = hit.transform.gameObject;
                            if (hitObject.tag.Equals("Player") || (hitObject.GetComponent<ReactiveObject>() != null && hitObject.GetComponent<ReactiveObject>().IsAttracted()))
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

                    if (speed < 2)
                    {
                        rb.useGravity = false;
                        rb.isKinematic = true;
                        _agent.enabled = true;

                        currentStateBuildUp = 0f;
                        _currentState = DroneState.Chasing;
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
}
