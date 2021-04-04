using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneV2 : MonoBehaviour, ReactiveObject
{
    public float movementSpeed;
    public float attackDistance = 5f;
    public float triggerDistance = 20f;

    private bool isTriggered;
    private bool isPlayerAffected;

    private Transform playerTransform;
    private GameObject player;
    Rigidbody rb;
    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        isTriggered = false;
        isPlayerAffected = false;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        player = GameObject.Find("Player");
        playerTransform = player.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isPlayerAffected)
        {
            float playerDistance = (transform.position - playerTransform.position).magnitude;
            if (playerDistance <= triggerDistance && !isTriggered)
            {
                isTriggered = true;
            }
            else
            {
                isTriggered = false;
            }

            if (isTriggered)
            {
                RaycastHit hit;
                Vector3 lastPosition = new Vector3();
                Vector3 direzione = new Vector3();

                transform.LookAt(playerTransform.position);

                if (Physics.Raycast(firePoint.position, firePoint.forward, out hit))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        lastPosition = hit.transform.position;
                        direzione = transform.position - playerTransform.position;
                        if (playerDistance >= attackDistance)
                        {
                            rb.velocity = -direzione.normalized * 4;
                        }
                        else
                        {
                            rb.velocity = direzione.normalized * 6;
                        }
                    }
                    else
                    {
                        direzione = transform.position - lastPosition;
                        rb.velocity = direzione.normalized * 4;
                    }
                }
            }

            Collider[] col = Physics.OverlapSphere(transform.position, 5.0f);
            Vector3 dist;
            if (col.Length != 0)
            {
                for (int i = 0; i < col.Length; i++)
                {
                    dist = (col[i].ClosestPoint(transform.position) - transform.position);
                    if (!isTriggered)
                    {
                        if (dist.magnitude > 1f)
                        {
                            rb.AddForce(dist);
                        }
                        else
                        {
                            rb.AddForce(-dist.normalized);
                        }
                    }
                    else
                    {
                        rb.AddForce(-dist.normalized);                  
                    }
                }
            }
        }
        else if(rb.velocity == new Vector3(0, 0, 0))
        {
            isPlayerAffected = false;
            rb.useGravity = false;
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
}
