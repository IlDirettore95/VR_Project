using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneV2 : MonoBehaviour, ReactiveObject
{
    public float movementSpeed;
    public float attackDistance = 5f;
    public float triggerDistance = 40f;

    private bool isTriggered;
    private bool isPlayerAffected;
    private bool playerOnSight;

    private Transform playerTransform;
    private GameObject player;
    Rigidbody rb;
    public Transform firePoint;
    Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        isTriggered = false;
        isPlayerAffected = false;
        playerOnSight = false;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        player = GameObject.Find("Player");
        playerTransform = player.transform;
        lastPosition = new Vector3(0, 0, 0);
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

            if (isTriggered)
            {
                RaycastHit hit;
                
                Vector3 direzione = new Vector3();

                transform.LookAt(playerTransform.position);

                if (Physics.Raycast(firePoint.position, firePoint.forward, out hit))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        playerOnSight = true;
                        lastPosition = playerTransform.position;
                    }
                    else
                    {
                        playerOnSight = false;
                    }
                }

                direzione = transform.position - lastPosition;
                if (playerDistance >= attackDistance)
                {
                    //rb.velocity = -direzione.normalized * 4;
                    rb.AddForce(-direzione.normalized * 4);
                    rb.velocity = rb.velocity.normalized * 4;
                }
                else if(playerDistance < attackDistance)
                {
                    //rb.velocity = direzione.normalized * 4;
                    //rb.AddForce(direzione.normalized * 4);
                    rb.velocity = new Vector3(0,0,0);
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
                            rb.AddForce(dist); //si allontana dall'oggetto
                            
                        }
                        else
                        {
                            rb.AddForce(-dist.normalized); //si avvicina all'oggetto
                            
                        }
                        
                        //rb.AddForce(1/dist.x, 1 / dist.y, 1 / dist.z);
                    }
                    else
                    {
                        rb.AddForce(dist);                  
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<ReactiveObject>() != null)
        {
            isPlayerAffected = true;
            rb.useGravity = true;
            isTriggered = true;
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
