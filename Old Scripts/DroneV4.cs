﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneV4 : MonoBehaviour, IEnemy, ReactiveEnemy
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
    }

    void FixedUpdate()
    {
        if (dead)
        {
            Die();
        }
        if (!isPlayerAffected)
        {
            float playerDistance = (transform.position - playerTransform.position).magnitude;
            if (playerDistance <= triggerPlayerDistance && !triggered)
            {
                triggered = true;
                //enemyManager.TriggerArea(areaID);
                nextTimeFire = Time.time + fireCooldown;
            }

            Vector3 direzione;

            if (!triggered)
            {
                Scan(transform.forward);
                Scan(-transform.forward);
                Scan(transform.up);
                Scan(-transform.up);
                Scan(transform.right);
                Scan(-transform.right);
            }
            else
            {
                transform.LookAt(playerTransform.position);
                lastPosition = playerTransform.position;

                if (playerDistance >= attackDistance)
                {
                    direzione = transform.position - lastPosition;
                    rb.AddForce(-direzione);
                    rb.velocity = rb.velocity.normalized * triggeredSpeed;
                }
                else if (playerDistance < attackDistance)
                {
                    //rb.velocity = Vector3.zero;
                    if (Time.time >= nextTimeFire)
                    {
                        Shoot();
                        nextTimeFire = Time.time + fireCooldown;
                    }
                }
            }
        }
        else if (rb.velocity.magnitude <= 1.5)
        {
            isPlayerAffected = false;
            rb.useGravity = false;
        }
        else
        {
            speed = rb.velocity.magnitude;
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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.transform.gameObject.tag.Equals("Player"))
            {
                Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReactiveObject collider = collision.gameObject.GetComponent<ReactiveObject>();
        if (collider != null)
        {
            Rigidbody colliderRb = collision.gameObject.GetComponent<Rigidbody>();
            if (colliderRb.velocity != rb.velocity && colliderRb.velocity.magnitude > 10)
            {
                isPlayerAffected = true;
                rb.useGravity = true;
                triggered = true;
                float damage = colliderRb.mass * colliderRb.velocity.magnitude;
                Debug.Log("Damage = " + damage);
                Hurt(damage);
            }
        }
        else
        {
            float damage = rb.mass * speed;
            Debug.Log("Damage = " + damage);
            Hurt(damage);
        }
    }

    public void Hurt(float damage)
    {
        _health -= damage;
        if (_health <= 0) dead = true;
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
        isPlayerAffected = true;
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
        isPlayerAffected = false;
        rb.useGravity = false;
    }

    public void ReactToLaunching(float launchingSpeed)
    {
        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.velocity = Vector3.zero;
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

}