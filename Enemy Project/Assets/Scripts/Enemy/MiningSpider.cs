using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MiningSpider : Enemy
{
    private bool exploding = false;

    //Explosion
    public float explosionDamage;
    public float explosionCooldown;
    private float nextTimeExplosion;
    public float explosionRadius;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        playerTransform = GameObject.Find("Player").transform;
        _playerStatus = playerTransform.GetComponent<PlayerStatus>();
        _health = MaxHealth;
        target = GameObject.Find("ObjectGrabber").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_health == 0)
        {
            Explode();
        }
        if (!isPlayerAffected)
        {
            float playerDistance = (transform.position - playerTransform.position).magnitude;
            if (playerDistance <= triggerPlayerDistance && !triggered)
            {
                triggered = true;
                //enemyManager.TriggerArea(areaID);
            }
            else if(playerDistance > attackDistance && exploding)
            {
                exploding = false;
                _agent.isStopped = false;
            }
            if (!triggered)
            {

            }
            else if(triggered && !exploding)
            {
                //Is no more calm
                //Debug.Log("TRIGGERED");
                //Debug.Log("rb velocity = " + rb.velocity.magnitude);

                //Lerping speed to triggered speed
                speed = Mathf.Lerp(speed, triggeredSpeed, triggeredSpeedBuildUp * Time.deltaTime);

                //Finding the player
                _agent.destination = playerTransform.position;
                //transform.LookAt(_player.position);

                //Follow the player
                if (_agent.remainingDistance <= attackDistance)
                {
                    //Debug.Log("EXPLODE");
                    exploding = true;
                    nextTimeExplosion = Time.time + explosionCooldown;
                }
            }
            if (exploding)
            {
                //Desabling NavMesh
                _agent.isStopped = true;
                if (Time.time >= nextTimeExplosion && playerDistance < attackDistance)
                {
                    Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRadius);
                    for (int i = 0; i < collisions.Length; i++)
                    {
                        if (collisions[i].gameObject.GetComponent<PlayerStatus>() != null)
                        {
                            //The explosion has hit the player
                            _playerStatus.Hurt(explosionDamage);
                            Debug.Log("Colpito! Ora hai solo " + _playerStatus.GetHealth() + " di vita.");
                        }
                        else if (collisions[i].gameObject.GetComponent<ReactiveEnemy>() != null)
                        {
                            collisions[i].gameObject.GetComponent<ReactiveEnemy>().ReactToExplosion(explosionDamage);
                        }
                    }
                    Die();
                }

            }
        }
        else if (rb.velocity == Vector3.zero)
        {
            isPlayerAffected = false;
            rb.useGravity = false;
            rb.isKinematic = true;
            _agent.enabled = true;
        }
        else
        {
            speed = rb.velocity.magnitude;
        }
    }

    private void Explode()
    {
        exploding = true;
        idle = false;
        walking = false;
        triggered = false;
    }

    public override void ReactToExplosion(float damage)
    {
        Explode();
        Debug.Log("Sono esploso per colpa di un Ragno!");
    }

    public override void ReactToAttraction(float attractionSpeed)
    {
        _agent.enabled = false;
        
        rb.isKinematic = false;
        isPlayerAffected = true;
        rb.useGravity = false;
        rb.freezeRotation = true;

        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);
    }

}
