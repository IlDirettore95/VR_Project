﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class DroneV5 : MonoBehaviour, IEnemy, ReactiveEnemy
{
    //Enemy manager
    private EnemiesManager enemyManager;
    public int areaID;
    public int enemyID;

    //Enemy state
    private bool dead = false;
    public enum DroneState
    {
        Guard,
        Chase,
        Throwed,
        Attracted,
        StartingSearch,
        Search,
        Patrol,
        Dead
    }

    //Enemy status
    public float MaxHealth;
    private float _health;

    //Speed
    public float walkingSpeed;
    public float triggeredSpeed;
    public float walkingSpeedBuildUp;
    public float triggeredSpeedBuildUp;

    //Trigger
    public float triggerPlayerDistance;
    public float triggerEnemyDistance;

    //Attack
    public float attackDistance;
    public float attackDamage;
    public float fireCooldown;
    private float nextTimeFire;
    public Transform firePoint;
    public GameObject projectilePrefab;

    //AI
    private Transform playerTransform;
    private GameObject player;
    private Rigidbody rb;
    private Vector3 lastPlayerPosition;
    private PlayerStatus _playerStatus;
    private NavMeshAgent _agent;
    private Vector3 startPos;
    private Vector3 targetPos;
    private readonly float posBuildUp = 1f;
    private float pos;
    private DroneState _currentState;

    //Player Interaction
    private Transform target;
    private float speed;  //Used to calculate damage when the enemy is launched by the player against static objects

    // Start is called before the first frame update
    void Start()
    {
        _health = MaxHealth;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        player = GameObject.Find("Player");
        playerTransform = player.transform;
        //lastPlayerPosition = Vector3.zero;

        target = GameObject.Find("ObjectGrabber").transform;

        enemyManager = GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>();

        _agent.GetComponent<NavMeshAgent>();
        _agent.enabled = false;
    }

    void FixedUpdate()
    {
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);

        if(dead)
        {
            Die();
        }

        switch (_currentState)
        {
            case DroneState.Dead:
                {
                    Die();

                    break;
                }
            case DroneState.Guard:
                {
                    Scan(transform.forward);
                    Scan(-transform.forward);
                    Scan(transform.up);
                    Scan(-transform.up);
                    Scan(transform.right);
                    Scan(-transform.right);

                    if (playerDistance <= triggerPlayerDistance)
                    {
                        _currentState = DroneState.Chase;
                        //enemyManager.TriggerArea(areaID);
                        nextTimeFire = Time.time + fireCooldown;
                        lastPlayerPosition = playerTransform.position;
                    }

                    break;
                }
            case DroneState.Patrol:
                {
                    break;
                }
            case DroneState.Chase:
                {
                    Vector3 direzione;

                    transform.LookAt(playerTransform.position);

                    Scan(transform.forward);
                    Scan(-transform.forward);
                    Scan(transform.up);
                    Scan(-transform.up);
                    Scan(transform.right);
                    Scan(-transform.right);

                    RaycastHit hit;
                    if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit))
                    {
                        if (hit.transform.gameObject.tag.Equals("Player") || hit.transform.gameObject.GetComponent<ReactiveObject>() != null)
                        {

                            if (playerDistance >= attackDistance)
                            {
                                direzione = transform.position - playerTransform.position;
                                rb.AddForce(-direzione.normalized * triggeredSpeed);
                                rb.velocity.Set(rb.velocity.x, rb.velocity.y, triggeredSpeed);
                            }
                            else if (playerDistance < attackDistance)
                            {
                                rb.velocity = Vector3.zero;

                                if (Time.time >= nextTimeFire)
                                {
                                    Shoot();
                                    nextTimeFire = Time.time + fireCooldown;
                                }
                            }
                        }
                        else
                        {
                            FindNavMesh();
                        }
                    }

                    break;
                }
            case DroneState.StartingSearch:
                {
                    //Debug.Log("Starting Search");

                    if (transform.position.Equals(targetPos))
                    {
                        _agent.enabled = true;
                        _currentState = DroneState.Search;
                    }
                    else
                    {
                        transform.LookAt(playerTransform.position);

                        RaycastHit hit;
                        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit))
                        {
                            if (hit.transform.gameObject.tag.Equals("Player"))
                            {
                                rb.isKinematic = false;
                                _currentState = DroneState.Chase;
                            }
                            else
                            {
                                pos += posBuildUp * Time.deltaTime;
                                transform.position = Vector3.Lerp(startPos, targetPos, pos);
                            }
                        }
                    }

                    break;
                }
            case DroneState.Search:
                {
                    //Debug.Log("Search");

                    Vector3 direzione = -(transform.position - playerTransform.position).normalized;

                    if (_agent.isOnOffMeshLink)
                    {
                        _agent.velocity = _agent.velocity.normalized;
                    }
                    
                    RaycastHit hit;
                    if (Physics.SphereCast(transform.position, 0.3f, direzione, out hit, 50f))
                    {
                        if (hit.transform.gameObject.tag.Equals("Player"))
                        {
                            _agent.enabled = false;
                            rb.isKinematic = false;
                            _currentState = DroneState.Chase;
                        }
                        else
                        {
                            _agent.destination = playerTransform.position;

                            if (_agent.hasPath)
                            {
                                lastPlayerPosition = _agent.destination;
                            }
                            else
                            {
                                _agent.destination = lastPlayerPosition;
                            }
                        }
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
                    if (rb.velocity.magnitude < 2)
                    {
                        rb.useGravity = false;

                        FindNavMesh();
                    }
                    else
                    {
                        speed = rb.velocity.magnitude;
                    }

                    break;
                }
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

    private void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
    }

    private void FindNavMesh()
    {
        //Debug.Log("FindNavMesh");

        startPos = transform.position;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, 5);
        foreach(Collider col in hits)
        {
            if (col.gameObject.name.Equals("Floor") && col.transform.position.y <= transform.position.y)
            {
                targetPos = col.ClosestPoint(transform.position) + transform.up * _agent.baseOffset;
                break;
            }
        }
        
        rb.isKinematic = true;
        pos = 0;
        _currentState = DroneState.StartingSearch;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReactiveObject collider = collision.gameObject.GetComponent<ReactiveObject>();
        if (collider != null)
        {
            Rigidbody colliderRb = collision.gameObject.GetComponent<Rigidbody>();
            if (colliderRb.velocity != rb.velocity && colliderRb.velocity.magnitude > 10)
            {
                _agent.enabled = false;
                rb.isKinematic = false;
                rb.useGravity = true;

                float damage = colliderRb.mass * colliderRb.velocity.magnitude;
                //Debug.Log("Damage = " + damage);
                Hurt(damage);

                _currentState = DroneState.Throwed;
            }
        }
        else
        {
            float damage = rb.mass * speed;
            //Debug.Log("Damage = " + damage);
            Hurt(damage);
        }
    }

    public void Hurt(float damage)
    {
        if (!dead && damage >= 1)
        {
            _health -= damage;
            if (_health <= 0) dead = true;
        }
    }

    protected void Die()
    {
        Debug.Log("Nemico morto");
        enemyManager.CollectEnemy(gameObject);
    }

    public void Revive()
    {
        _health = MaxHealth;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;

        _currentState = DroneState.Guard;
    }

    public void BeTriggered()
    {
        throw new System.NotImplementedException();
    }

    public void MoveTo(Vector3 point)
    {
        throw new System.NotImplementedException();
    }

    public void Patrol(Vector3[] path)
    {
        throw new System.NotImplementedException();
    }

    public void ReactToAttraction(float attractionSpeed)
    {
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);

        _currentState = DroneState.Attracted;
    }

    public void ReactToRepulsing()
    {
        throw new System.NotImplementedException();
    }

    public void ReactToReleasing()
    {
        rb.useGravity = true;
        rb.freezeRotation = false;

        _currentState = DroneState.Throwed;
    }

    public void ReactToLaunching(float launchingSpeed)
    {
        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.velocity += target.forward * 10;
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);

        _currentState = DroneState.Throwed;
    }

    public void ReactToIncreasing()
    {
        throw new System.NotImplementedException();
    }

    public void ReactToDecreasing()
    {
        throw new System.NotImplementedException();
    }

    public void ReactToExplosion(float damage)
    {
        Hurt(damage);
    }

    public int GetID()
    {
        return enemyID;
    }

    public void SetID(int id)
    {
        enemyID = id;
    }

    public int GetAreaID()
    {
        return areaID;
    }

    public void SetAreaID(int id)
    {
        areaID = id;
    }

    public void Initialize()
    {
        _health = MaxHealth;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        player = GameObject.Find("Player");
        playerTransform = player.transform;
        lastPlayerPosition = Vector3.zero;

        target = GameObject.Find("ObjectGrabber").transform;

        enemyManager = GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;

        _currentState = DroneState.Guard;
    }

    public bool IsDestroyed()
    {
        return false;
    }
}
