using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class SimpleIA1 : MonoBehaviour
{
    public float movementSpeed = 4f;
    public float attackDistance = 3f;
    public float npcHP = 100;
    public float npcDamage = 5;
    public float attackRate = 0.5f;
    float nextAttackTime = 0;
    public Transform firePoint;

    [HideInInspector]
    public Transform playerTransform;
    private GameObject player;

    NavMeshAgent agent;
    Rigidbody r;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        r = GetComponent<Rigidbody>();
        r.useGravity = false;
        r.isKinematic = true;
        player = GameObject.Find("Player");
        playerTransform = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance - attackDistance < 0.01f)
        {
            if (Time.time > nextAttackTime)
            {
                nextAttackTime = Time.time + attackRate;

                //Attack
                RaycastHit hit;
                if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, attackDistance))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        Debug.DrawLine(firePoint.position, firePoint.position + firePoint.forward * attackDistance, Color.cyan);
                    }
                }
            }
        }

        //Move towardst he player
        agent.destination = playerTransform.position;
        //Always look at player
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, playerTransform.position.z));
        //Gradually reduce rigidbody velocity if the force was applied by the bullet
        r.velocity *= 0.99f;
    }

    public void Hurt(float damage)
    {
        npcHP -= damage;
        Debug.Log("Enemy hurted! HP: " + npcHP);
        if (npcHP <= 0)
            Destroy(gameObject);
    }
}
