using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        enemyManager = GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>();
    }

    public override void Initialize()
    {
        _agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        playerTransform = GameObject.Find("Player").transform;
        _playerStatus = playerTransform.GetComponent<PlayerStatus>();
        _health = MaxHealth;
        target = GameObject.Find("ObjectGrabber").transform;
        enemyManager = GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);

        //Triggered
        if (triggered)
        {
            if (dead)
            {
                Explode();
                Die();
            }
            else if (exploding)
            {
                //Desabling NavMesh
                if (!attracted && !throwed)
                {
                    _agent.isStopped = true;
                }

                if (Time.time >= nextTimeExplosion)
                {
                    Explode();
                }
                else if(playerDistance > attackDistance)
                {
                    exploding = false;
                    _agent.isStopped = false;
                }
            }
            else if (throwed || attracted)
            {
                if (throwed && rb.velocity == Vector3.zero)
                {
                    throwed = false;
                    rb.useGravity = false;
                    rb.isKinematic = true;
                    _agent.enabled = true;
                }
                else
                {
                    speed = rb.velocity.magnitude;
                }
            }
            else //Not playerAffected
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
                if (playerDistance <= attackDistance)
                {
                    Debug.Log("EXPLODE");
                    exploding = true;
                    nextTimeExplosion = Time.time + explosionCooldown;
                }
            }
        }
        //Not triggered
        else
        {
            if (playerDistance <= triggerPlayerDistance)
            {
                triggered = true;
                //enemyManager.TriggerArea(areaID);
            }
        }
    }

    private void Explode()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int i = 0; i < collisions.Length; i++)
        {
            PlayerStatus ps = collisions[i].gameObject.GetComponent<PlayerStatus>();
            if (ps != null && ps.IsAlive())
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
    }

    public override void Revive()
    {
        idle = true;
        walking = false;
        triggered = false;
        throwed = false;
        attracted = false;
        dead = false;
        exploding = false;

        _health = MaxHealth;
        rb.useGravity = false;
        rb.isKinematic = true;
        _agent.enabled = true;
    }

}
