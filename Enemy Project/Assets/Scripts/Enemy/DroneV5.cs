using System.Collections;
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
    private bool idle = true;
    private bool walking = false;
    private bool triggered = false;
    private bool isPlayerAffected = false;
    private bool dead = false;
    private bool attracted = false;
    private bool throwed = false;
    private bool searching = false;
    private bool startSearch = false;


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
    private Vector3 lastPosition;
    private PlayerStatus _playerStatus;
    private NavMeshAgent _agent;
    private Vector3 lastPos;
    private Vector3 targetPos;
    private readonly float posBuildUp = 1f;
    private float pos;
    private float nextTimeSearchLook = 2;

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
        lastPosition = Vector3.zero;

        target = GameObject.Find("ObjectGrabber").transform;

        enemyManager = GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>();

        _agent.GetComponent<NavMeshAgent>();
        _agent.enabled = false;
    }

    void FixedUpdate()
    {
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);

        //Triggered
        if (triggered)
        {
            if (dead)
            {
                Die();
            }
            else if (throwed)
            {
                if (rb.velocity.magnitude < 2)
                {
                    throwed = false;
                    rb.useGravity = false;
                }
                else
                {
                    speed = rb.velocity.magnitude;
                }
            }
            else if (attracted)
            {

            }
            else if (startSearch)
            {
                if(transform.position.Equals(targetPos))
                {
                    _agent.enabled = true;
                    searching = true;
                    startSearch = false;
                    _agent.destination = playerTransform.position;

                    //nextTimeSearchLook = Time.time + 2f;
                }
                else
                {
                    transform.LookAt(playerTransform.position);
                    RaycastHit hit;
                    if (Physics.SphereCast(transform.position, 5f, transform.forward, out hit))
                    {
                        if (hit.transform.gameObject.tag.Equals("Player"))
                        {
                            startSearch = false;
                            rb.isKinematic = false;
                        }
                        else
                        {
                            pos += posBuildUp * Time.deltaTime;
                            transform.position = Vector3.Lerp(lastPos, targetPos, pos);
                        }
                    }
                }
            }
            else if (searching)
            {
                Vector3 direzione = -(transform.position - playerTransform.position).normalized;

                if (_agent.isOnOffMeshLink)
                {
                    _agent.velocity = _agent.velocity.normalized;
                }

                RaycastHit hit;
                if (Physics.SphereCast(transform.position, 0.5f, direzione, out hit, 50f))
                {
                    if (hit.transform.gameObject.tag.Equals("Player"))
                    {
                        searching = false;
                        _agent.enabled = false;
                        rb.isKinematic = false;
                    }
                    else
                    {
                        _agent.destination = playerTransform.position;

                        if (!_agent.hasPath)
                        {
                            searching = false;
                            _agent.enabled = false;
                            rb.isKinematic = false;
                        }
                    }
                }
            }
            else //Not playerAffected
            {
                Vector3 direzione;

                transform.LookAt(playerTransform.position);
                lastPosition = playerTransform.position;

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
                            direzione = transform.position - lastPosition;
                            rb.AddForce(-direzione.normalized * triggeredSpeed);
                            rb.velocity.Set(rb.velocity.x, rb.velocity.y, triggeredSpeed);
                        }
                        else if (playerDistance < attackDistance)
                        {
                            //rb.velocity.Set(rb.velocity.x, rb.velocity.y, 0);
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
                        FindPlayer();
                    }
                }
            }
        }
        //Not triggered
        else
        {
            if (playerDistance <= triggerPlayerDistance)
            {
                triggered = true;
                //enemyManager.TriggerArea(areaID);
                nextTimeFire = Time.time + fireCooldown;
            }

            Scan(transform.forward);
            Scan(-transform.forward);
            Scan(transform.up);
            Scan(-transform.up);
            Scan(transform.right);
            Scan(-transform.right);
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

    private void FindPlayer()
    {
        lastPos = transform.position;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, 10);
        foreach(Collider col in hits)
        {
            if (col.gameObject.name.Equals("Floor") && col.transform.position.y <= transform.position.y)
            {
                targetPos = col.ClosestPoint(transform.position) + transform.up * 2;
                break;
            }
        }

        rb.isKinematic = true;
        startSearch = true;
        pos = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReactiveObject collider = collision.gameObject.GetComponent<ReactiveObject>();
        if (collider != null)
        {
            Rigidbody colliderRb = collision.gameObject.GetComponent<Rigidbody>();
            if (colliderRb.velocity != rb.velocity && colliderRb.velocity.magnitude > 10)
            {
                throwed = true;
                searching = false;
                startSearch = false;
                _agent.enabled = false;
                rb.isKinematic = false;
                rb.useGravity = true;
                triggered = true;
                float damage = colliderRb.mass * colliderRb.velocity.magnitude;
                //Debug.Log("Damage = " + damage);
                Hurt(damage);
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
            triggered = true;
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
        idle = true;
        walking = false;
        triggered = false;
        isPlayerAffected = false;
        dead = false;

        _health = MaxHealth;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
    }

    public void BeTriggered()
    {
        triggered = true;
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
        attracted = true;
        throwed = false;
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);
    }

    public void ReactToRepulsing()
    {
        throw new System.NotImplementedException();
    }

    public void ReactToReleasing()
    {
        attracted = false;
        throwed = true;
        rb.useGravity = true;
        rb.freezeRotation = false;
    }

    public void ReactToLaunching(float launchingSpeed)
    {
        attracted = false;
        throwed = true;
        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        //rb.velocity = Vector3.zero;
        rb.velocity += target.forward * 10;
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);
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
        lastPosition = Vector3.zero;

        target = GameObject.Find("ObjectGrabber").transform;

        enemyManager = GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
    }

    public bool IsDestroyed()
    {
        return false;
    }
}
