using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiningSpider : Enemy
{
    //This enum are mining spider possible states
    public enum SpiderState
    {
        Guarding,
        Patrolling,
        Chasing,
        Attracted,
        Throwed,
        Exploding
    }

    //MiningSpider current state
    private SpiderState _currentState;

    //Explosion
    public float explosionDamage;
    public float explosionCooldown;
    private float nextTimeExplosion;
    public float explosionRadius;
    public float explosionImpact;

    //Throwed stunning
    public float stunningDuration;
    private float nextTimeStunning;

    //Exploding procedure animation
    private LightFlickering fLight1;
    private LightFlickering fLight2;

    //MiningSpider lights
    [SerializeField] private Light light1;
    [SerializeField] private Light light2;

    //Explosion
    [SerializeField] private ParticleSystem explosion;

    //Audio
    private AudioSource timer;

    //Rotation of sight before moving
    public float sightRotationThreashold;
    public float rotationSpeed;

    //Animation
    SpiderAnimationController _animController;

    public SpiderState GetState() => _currentState;
    

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.Find("Player").transform;
        _playerStatus = playerTransform.GetComponent<PlayerStatus>();
        _animController = GetComponent<SpiderAnimationController>();

        rb.useGravity = false;
        rb.isKinematic = true;
        _health = MaxHealth;
        target = GameObject.Find("ObjectGrabber").transform;

        fLight1 = GetComponents<LightFlickering>()[0];
        fLight2 = GetComponents<LightFlickering>()[1];

        timer = GetComponent<AudioSource>();

        isAlive = true;

        _currentState = SpiderState.Guarding;
    }

    /*
    public override void Initialize()
    {
        _agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        playerTransform = GameObject.Find("Player").transform;
        _playerStatus = playerTransform.GetComponent<PlayerStatus>();
        _health = MaxHealth;
        target = GameObject.Find("ObjectGrabber").transform;
        enemyManager = GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>();
    }
    */

    // Update is called once per frame
    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);

        switch (_currentState)
        {
            case SpiderState.Guarding:
                { 
                    if (patrollingPoints.Length >= 2 && Time.time >= nextTimePatrol)
                    {
                        ChangeDestination();
                        Patrol();
                        
                        _currentState = SpiderState.Patrolling;
                    }

                    FindPlayer();

                    break;
                }
            case SpiderState.Patrolling:
                {
                    if(isAtDestination())
                    {
                        nextTimePatrol = Time.time + guardingCooldown;
                        _currentState = SpiderState.Guarding;
                    }

                    FindPlayer();

                    break;
                }
            
            case SpiderState.Chasing:
                {
                    //Passing to Exploding State
                    if (playerDistance <= attackDistance)
                    {
                        nextTimeExplosion = Time.time + explosionCooldown;
                        _agent.isStopped = true;
                        _currentState = SpiderState.Exploding;

                        //Enabling animation
                        fLight1.enabled = true;
                        fLight2.enabled = true;

                        //Audio
                        timer.Play();

                        _animController.UpdateAnimation();
                    }
                    else
                    {
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
                            //Lerping speed to triggered speed
                            currentStateBuildUp += triggeredSpeedBuildUp * Time.deltaTime;
                            speed = Mathf.Lerp(walkingSpeed, triggeredSpeed, currentStateBuildUp);
                            _agent.speed = speed;

                            //Finding the player
                            _agent.destination = playerTransform.position;
                            _agent.isStopped = false;
                        }
                    }    
                    break;
                }
            case SpiderState.Attracted:
                {
                    speed = rb.velocity.magnitude;
                    //Control if it is time to explode
                    if (Time.time >= nextTimeExplosion)
                    {
                        Explode();
                    }

                    break;
                }
            case SpiderState.Throwed:
                {
                    speed = rb.velocity.magnitude;

                    if (speed == 0f && Time.time >= nextTimeStunning)
                    {
                        rb.useGravity = false;
                        rb.isKinematic = true;
                        _agent.enabled = true;

                        currentStateBuildUp = 0f;
                        _currentState = SpiderState.Chasing;
                        _animController.UpdateAnimation();
                    }
                    break;
                }
            case SpiderState.Exploding:
                {
                    //Exploding procedure start

                    //Control if it is time to explode
                    if (Time.time >= nextTimeExplosion)
                    {
                        Explode();
                    }
                    //Passing to Triggered State
                    else if (playerDistance > attackDistance)
                    {
                        //Disenabling animation
                        fLight1.enabled = false;
                        fLight2.enabled = false;

                        //Audio
                        timer.Stop();

                        _agent.isStopped = false;
                        _currentState = SpiderState.Chasing;
                        _animController.UpdateAnimation();
                    }
                    break;
                }
        }
    }

    private void Explode()
    {
        //Kill the spider
        isAlive = false;

        Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int i = 0; i < collisions.Length; i++)
        {
            ReactiveEntity target = collisions[i].gameObject.GetComponent<ReactiveEntity>();
            if (target != null && collisions[i].gameObject != this.gameObject)
            {
                target.ReactToExplosion(explosionDamage, explosionImpact, transform.position, explosionRadius);
            }
        }

        //Audio
        timer.Stop();

        //ParticleSystem
        ParticleSystem ex = Instantiate(explosion, transform.position, transform.rotation);

        Destroy(this.gameObject);
    }

    public override void ReactToAttraction(float attractionSpeed)
    {
        //Just the first time the spider become attracted
        if(!attracted)
        {
            //If the spider wasn't in exploding state, this will set up the exploding procedure
            if (!_currentState.Equals(SpiderState.Exploding))
            {
                nextTimeExplosion = Time.time + explosionCooldown;

                //Enabling animation
                fLight1.enabled = true;
                fLight2.enabled = true;

                //Audio
                timer.Play();
            }
            _currentState = SpiderState.Attracted;
            _animController.UpdateAnimation();
        }

        base.ReactToAttraction(attractionSpeed);
    }

    public override void ReactToReleasing()
    {
        base.ReactToReleasing();

        //Disabling animation
        fLight1.enabled = false;
        fLight2.enabled = false;

        //Audio
        timer.Stop();

        nextTimeStunning = Time.time + stunningDuration;
        _currentState = SpiderState.Throwed;
        _animController.UpdateAnimation();
    }

    public override void ReactToLaunching(float launchingSpeed)
    {
        base.ReactToLaunching(launchingSpeed);

        //Disabling animation
        fLight1.enabled = false;
        fLight2.enabled = false;

        //Audio
        timer.Stop();

        nextTimeStunning = Time.time + stunningDuration;
        _currentState = SpiderState.Throwed;
        _animController.UpdateAnimation();
    }

    public override void ReactToExplosion(float damage, float power, Vector3 center, float radius)
    {
        if (isAlive) Explode();    
    }

    public override void Hurt(float damage)
    {
        if(isAlive)
        {
            base.Hurt(damage);

            if (_health == 0) Explode();
        }        
    }

    //Handles collision damage
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody colliderRb = collision.gameObject.GetComponent<Rigidbody>();
        if (colliderRb != null)
        {
            if (collision.relativeVelocity.magnitude > impactVelocityThreashold)
            {
                if(_currentState != SpiderState.Throwed)
                {
                    _agent.enabled = false;
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.AddForce(collision.relativeVelocity, ForceMode.Impulse);
                    nextTimeStunning = Time.time + stunningDuration;
                    _currentState = SpiderState.Throwed;
                    _animController.UpdateAnimation();
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
        if (_currentState == SpiderState.Patrolling || _currentState == SpiderState.Guarding)
        {
            StartCoroutine(TriggerArea(areaID));
            currentStateBuildUp = 0f;

            _currentState = SpiderState.Chasing;
            _animController.UpdateAnimation();
        }
    }
}
