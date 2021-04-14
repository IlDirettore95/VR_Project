using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneV4 : Enemy
{
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
                    rb.velocity = Vector3.zero;
                    if (Time.time >= nextTimeFire)
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
        Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
    }

    public override void ReactToAttraction(float attractionSpeed)
    {
        isPlayerAffected = true;
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);
    }

    public override void ReactToReleasing()
    {
        isPlayerAffected = false;
        rb.useGravity = false;
    }

    public override void ReactToLaunching(float launchingSpeed)
    {
        rb.useGravity = true;
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);
    }
}
