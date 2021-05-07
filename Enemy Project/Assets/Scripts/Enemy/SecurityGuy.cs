using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecurityGuy : Enemy
{
    //Enemy state
    public enum SecurityGuyState
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
    private SecurityGuyState _currentState;

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

        _currentState = SecurityGuyState.Guarding;
    }

    // Update is called once per frame
    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);

        if (!isAlive) _currentState = SecurityGuyState.Dead;

        switch (_currentState)  //Finite State Machine
        {
            case SecurityGuyState.Dead:
                {
                    Destroy(gameObject);

                    break;
                }
            case SecurityGuyState.Guarding:
                {
                    if (playerDistance <= triggerPlayerDistance)
                    {
                        nextTimeFire = Time.time + fireCooldown;
                        lastPlayerPosition = playerTransform.position;
                        TriggerArea(areaID);

                        _currentState = SecurityGuyState.Chasing;
                    }

                    break;
                }
            case SecurityGuyState.Patrolling:
                {
                    if (playerDistance <= triggerPlayerDistance)
                    {
                        nextTimeFire = Time.time + fireCooldown;
                        lastPlayerPosition = playerTransform.position;
                        TriggerArea(areaID);

                        _currentState = SecurityGuyState.Chasing;
                    }

                    break;
                }
            case SecurityGuyState.Chasing:
                {
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
                    else if (!_agent.isOnOffMeshLink)
                    {
                        Vector3 direzione = -(transform.position - playerTransform.position).normalized;

                        RaycastHit hit;
                        if (Physics.SphereCast(transform.position, 0.3f, direzione, out hit, 50f))
                        {
                            GameObject hitObject = hit.transform.gameObject;
                            if (hitObject.tag.Equals("Player") || (hitObject.GetComponent<ReactiveObject>() != null && hitObject.GetComponent<ReactiveObject>().IsAttracted()))
                            {
                                _agent.isStopped = true;
                                _currentState = SecurityGuyState.Attack;
                            }
                        }
                    }

                    break;
                }
            case SecurityGuyState.Attack:
                {
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
                        _currentState = SecurityGuyState.Chasing;
                    }

                    break;
                }
            case SecurityGuyState.Attracted:
                {
                    break;
                }
            case SecurityGuyState.Throwed:
                {
                    speed = rb.velocity.magnitude;

                    if (speed < 3)
                    {
                        rb.useGravity = false;
                        rb.velocity = Vector3.zero;

                        currentStateBuildUp = 0f;
                        _currentState = SecurityGuyState.Stunned;
                    }

                    break;
                }
            case SecurityGuyState.Stunned:
                {
                    _agent.enabled = true;
                    _currentState = SecurityGuyState.Chasing;

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
                if (_currentState == SecurityGuyState.Patrolling || _currentState == SecurityGuyState.Guarding)
                {
                    TriggerArea(areaID);
                }

                if (_currentState != SecurityGuyState.Throwed)
                {
                    _agent.enabled = false;
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.AddForce(collision.relativeVelocity, ForceMode.Impulse);
                    _currentState = SecurityGuyState.Throwed;
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

        _currentState = SecurityGuyState.Attracted;
    }

    public override void ReactToReleasing()
    {
        base.ReactToReleasing();

        _currentState = SecurityGuyState.Throwed;
    }

    public override void ReactToLaunching(float launchingSpeed)
    {
        base.ReactToLaunching(launchingSpeed);

        _currentState = SecurityGuyState.Throwed;
    }

    public override void Triggered()
    {
        if (_currentState == SecurityGuyState.Patrolling || _currentState == SecurityGuyState.Guarding)
        {
            _currentState = SecurityGuyState.Chasing;
        }
    }
}
