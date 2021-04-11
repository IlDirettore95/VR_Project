using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneV3 : MonoBehaviour, ReactiveObject, IEnemy
{
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
    public float attackDistance = 5f;
    public float attackDamage;

    private Transform playerTransform;
    private GameObject player;
    Rigidbody rb;
    public Transform firePoint;
    Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
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
        if (!isPlayerAffected)
        {
            float playerDistance = (transform.position - playerTransform.position).magnitude;
            if (playerDistance <= triggerPlayerDistance && !triggered)
            {
                triggered = true;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<ReactiveObject>() != null)
        {
            isPlayerAffected = true;
            rb.useGravity = true;
            triggered = true;
        }
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

    public void Hurt(float damage)
    {
        _health -= damage;
        if (_health < 0) _health = 0;
    }

    public void TriggerNearbyEnemies()
    {
        
    }
}
