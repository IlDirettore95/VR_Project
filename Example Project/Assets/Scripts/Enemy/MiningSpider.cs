﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiningSpider : Enemy
{
    //MiningSpider current state
    private SpiderState _currentState;
    private bool exploding = false;

    //Explosion
    public float explosionDamage;
    public float explosionCooldown;
    private float nextTimeExplosion;
    public float explosionRadius;

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
        //enemyManager = GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>();

        fLight1 = GetComponents<LightFlickering>()[0];
        fLight2 = GetComponents<LightFlickering>()[1];

        timer = GetComponent<AudioSource>();

    }

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

    // Update is called once per frame
    void Update()
    {
        if(_currentState != SpiderState.Dead)
        {
            float playerDistance = Vector3.Distance(transform.position, playerTransform.position);

            switch (_currentState)
            {
                case SpiderState.NotTriggered:
                    {
                        if (playerDistance <= triggerPlayerDistance)
                        {
                            currentStateBuildUp = 0f;
                            _currentState = SpiderState.Triggered;
                        }
                        break;
                    }
                case SpiderState.Triggered:
                    {
                        //Following the player

                        //Lerping speed to triggered speed
                        currentStateBuildUp += triggeredSpeedBuildUp * Time.deltaTime;
                        speed = Mathf.Lerp(walkingSpeed, triggeredSpeed, currentStateBuildUp);
                        _agent.speed = speed;

                        //Finding the player
                        _agent.destination = playerTransform.position;

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

                        if (speed == 0f)
                        {
                            rb.useGravity = false;
                            rb.isKinematic = true;
                            _agent.enabled = true;

                            currentStateBuildUp = 0f;
                            _currentState = SpiderState.Triggered;
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
                            _currentState = SpiderState.Triggered;
                        }
                        break;
                    }
            }

        }

       
    }

    private void Explode()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int i = 0; i < collisions.Length; i++)
        {
            PlayerStatus ps = collisions[i].gameObject.GetComponent<PlayerStatus>();
            if (ps != null && ps.IsAlive())
            {
                //The explosion has hit the player
                _playerStatus.Hurt(explosionDamage);
            }
            else if (collisions[i].gameObject.GetComponent<ReactiveEntity>() != null)
            {
                collisions[i].gameObject.GetComponent<ReactiveEntity>().ReactToExplosion(explosionDamage);
            }
        }

        //Simulating the explosion
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(Vector3.up, ForceMode.Impulse);

        //Kill the spider

        //Disabling animation
        fLight1.enabled = false;
        fLight2.enabled = false;
        light1.enabled = false;
        light2.enabled = false;

        //Audio
        timer.Stop();

        //ParticleSystem
        ParticleSystem ex = Instantiate(explosion, transform.position, transform.rotation);

        _currentState = SpiderState.Dead;
    }

    public override void ReactToAttraction(float attractionSpeed)
    {
        //Just the first time the spider become attracted
        if(!_currentState.Equals(SpiderState.Dead) && !attracted)
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
        }

        base.ReactToAttraction(attractionSpeed);
    }

    public override void ReactToReleasing()
    {
        base.ReactToReleasing();

        if (!_currentState.Equals(SpiderState.Dead))
        {
            //Disabling animation
            fLight1.enabled = false;
            fLight2.enabled = false;

            //Audio
            timer.Stop();

            _currentState = SpiderState.Throwed;
        }
    }

    public override void ReactToLaunching(float launchingSpeed)
    {
        base.ReactToLaunching(launchingSpeed);

        if (!_currentState.Equals(SpiderState.Dead))
        {
            //Disabling animation
            fLight1.enabled = false;
            fLight2.enabled = false;

            //Audio
            timer.Stop();

            _currentState = SpiderState.Throwed;
        }        
    }

    public override void Hurt(float damage)
    {
        base.Hurt(damage);

        if (_health == 0) Explode();
    }

    //This enum are mining spider possible states
    public enum SpiderState
    {
        NotTriggered,
        Triggered,
        Attracted,
        Throwed,
        Exploding,
        Dead
    }
}
