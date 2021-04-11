using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneV3 : MonoBehaviour, ReactiveObject
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

    private RaycastHit[] hits;
    public Transform[] firePoints;

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

        //hits = new RaycastHit[6];
        //firePoints = new Transform[6];
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

            Vector3 direzione;

            if (!isTriggered)
            {
                RaycastHit hit;
                float dist;
                
                for (int i = 0; i < firePoints.Length; i++)
                {
                    if (Physics.Raycast(firePoints[i].position, firePoints[i].forward, out hit))
                    {
                        
                        ///dist = Vector3.Distance(transform.position, hit.transform.position);
                        if(hit.distance < 6f)
                        {
                            Debug.Log("dist: " + hit.distance);
                            //direzione = Vector3.MoveTowards(transform.position, hit.point, 1f);
                            //rb.MovePosition(direzione);
                            //direzione = hit.transform.position - transform.position;
                            direzione = hit.point - transform.position;
                            rb.AddForce(-direzione.normalized * 5);
                        }
                        else if(hit.distance < 3f)
                        {
                            Debug.Log("dist: " + hit.distance);
                            direzione = hit.point - transform.position;
                            rb.AddForce(-direzione.normalized * 10);
                        }
                    }
                }
            }
            else
            {
                transform.LookAt(playerTransform.position);
                lastPosition = playerTransform.position;
                
                if (playerDistance >= attackDistance)
                {
                    direzione = transform.position - lastPosition;
                    rb.AddForce(-direzione);
                    rb.velocity = rb.velocity.normalized * 4;
                }
                else if (playerDistance < attackDistance)
                {
                    rb.velocity = Vector3.zero;
                }
            }
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
