using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneV2 : MonoBehaviour
{
    public float movementSpeed;
    public float attackDistance = 5f;
    public float triggerDistance = 20f;

    private bool isTriggered;

    private Transform playerTransform;
    private GameObject player;
    Rigidbody r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.useGravity = false;
        player = GameObject.Find("Player");
        playerTransform = player.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
                        r.AddForce(dist);
                    }
                    else
                    {
                        r.AddForce(dist * 4);
                    }
                }
                else
                {
                    r.velocity = -dist.normalized * 6;
                }
            }
        }

        float playerDistance = (transform.position - playerTransform.position).magnitude;
        float movementSpeed = playerDistance;
        Debug.Log("Player distance: " + playerDistance);
        if(playerDistance <= triggerDistance)
        {
            transform.LookAt(playerTransform.position);
            isTriggered = true;

            if (playerDistance >= attackDistance)
            {
                r.velocity = -(transform.position - playerTransform.position).normalized * 6;
            }

            if (playerDistance <= attackDistance)
            {
                r.velocity = (transform.position - playerTransform.position).normalized * 6;
            }
        }
        else
        {
            isTriggered = false;
        }
    }
}
