﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneV3 : MonoBehaviour, ReactiveObject, IEnemy
{
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
    Rigidbody rb;
    Vector3 lastPosition;

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
    }

    // Update is called once per frame
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
                    rb.velocity = Vector3.zero;
                    if(Time.time >= nextTimeFire)
                    {
                        Shoot();
                        nextTimeFire = Time.time + fireCooldown;
                    }
                }
            }
        }
        else if (rb.velocity == Vector3.zero)
        {
            isPlayerAffected = false;
            rb.useGravity = false;
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

        //if (rb.velocity.magnitude > triggeredSpeed) rb.velocity = Vector3.zero;
    }

    private void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReactiveObject collider = collision.gameObject.GetComponent<ReactiveObject>();
        if (collider != null)
        {
            Rigidbody colliderRb = collision.gameObject.GetComponent<Rigidbody>();
            if(colliderRb.velocity != rb.velocity && colliderRb.velocity.magnitude > 10)
            {
                isPlayerAffected = true;
                rb.useGravity = true;
                triggered = true;
                float damage = colliderRb.mass * colliderRb.velocity.magnitude;
                Debug.Log("Damage = " + damage);
                Hurt(damage);
            }
        }
    }

    public void Hurt(float damage)
    {
        _health -= damage;
        if (_health <= 0) dead = true;
    }

    private void Die()
    {
        Debug.Log("Drone morto");
        Destroy(gameObject);
    }

    public void ReactToAttraction(Vector3 target, float attractionSpeed)
    {
        isPlayerAffected = true;
        rb.useGravity = false;
        attractionSpeed *= Vector3.Distance(target, rb.position);
        if (rb.position.Equals(target)) return;
        else rb.position = Vector3.MoveTowards(rb.position, target, attractionSpeed);
    }

    public void ReactToRepulsing(Vector3 direction, float repulsingSpeed)
    {
        isPlayerAffected = true;
        rb.useGravity = true;
        rb.AddForce(direction * repulsingSpeed, ForceMode.Impulse);
    }

    public void ReactToReleasing()
    {
        isPlayerAffected = false;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
    }

    public void ReactToLaunching(Vector3 direction, float launchingSpeed)
    {
        rb.AddForce(direction * launchingSpeed, ForceMode.Impulse);
        rb.useGravity = true;
    }

    public void TriggerNearbyEnemies()
    {
        //@TODO
    }
}
