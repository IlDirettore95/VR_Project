using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IEnemy, ReactiveEnemy
{
    //Enemy manager
    protected EnemiesManager enemyManager;
    public int areaID;
    public int enemyID;

    //Enemy state
    protected bool idle = true;
    protected bool walking = false;
    protected bool triggered = false;
    protected bool isPlayerAffected = false;
    protected bool dead = false;

    //Enemy status
    public float MaxHealth;
    protected float _health;

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
    protected float nextTimeFire;
    public Transform firePoint;
    public GameObject projectilePrefab;

    //AI
    protected NavMeshAgent _agent;
    protected Transform playerTransform;
    protected GameObject player;
    protected Rigidbody rb;
    protected Vector3 lastPosition;
    protected PlayerStatus _playerStatus;

    //Player Interaction
    protected Transform target;
    protected float speed;  //Used to calculate damage when the enemy is launched by the player against static objects

    public void SetAreaID(int ID)
    {
        areaID = ID;
    }

    public void SetID(int id)
    {
        enemyID = id;
    }

    public int GetID()
    {
        return enemyID;
    }

    private void OnCollisionEnter(Collision collision)
    {
       ReactiveObject collider = collision.gameObject.GetComponent<ReactiveObject>();
        if (collider != null)
        {
            Rigidbody colliderRb = collision.gameObject.GetComponent<Rigidbody>();
            if (colliderRb.velocity != rb.velocity && colliderRb.velocity.magnitude > 10)
            {
                isPlayerAffected = true;
                _agent.enabled = false;
                rb.isKinematic = false;
                rb.useGravity = true;
                triggered = true;
                float damage = colliderRb.mass * colliderRb.velocity.magnitude;
                Debug.Log("Damage = " + damage);
                Hurt(damage);
                rb.AddForce(colliderRb.velocity.normalized * 30, ForceMode.Impulse);
                Debug.Log("forza: " + colliderRb.mass * colliderRb.velocity);
            }
        }
        else
        {
            float damage = rb.mass * speed;
            Debug.Log("Damage = " + damage);
            Hurt(damage);
        }
    }

    public void Hurt(float damage)
    {
        _health -= damage;
        if (_health <= 0) dead = true;
    }

    protected void Die()
    {
        Debug.Log("Nemico morto");
        enemyManager.CollectEnemy(gameObject);
    }

    public void Revive()
    {
        idle = true;
        walking = false;
        triggered = false;
        isPlayerAffected = false;
        dead = false;

        _health = MaxHealth;
        rb.useGravity = false;
    }

    public void BeTriggered()
    {
        triggered = true;
    }

    public virtual void MoveTo(Vector3 point)
    {
        throw new System.NotImplementedException();
    }

    public virtual void Patrol(Vector3[] path)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ReactToAttraction(float attractionSpeed)
    {
        _agent.enabled = false;
        rb.isKinematic = false;
        isPlayerAffected = true;
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);
    }

    public virtual void ReactToRepulsing()
    {
        throw new System.NotImplementedException();
    }

    public virtual void ReactToReleasing()
    {
        isPlayerAffected = false;
        rb.useGravity = false;
        rb.isKinematic = true;
        _agent.enabled = true;
    }

    public virtual void ReactToLaunching(float launchingSpeed)
    {
        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);
    }

    public virtual void ReactToIncreasing()
    {
        throw new System.NotImplementedException();
    }

    public virtual void ReactToDecreasing()
    {
        throw new System.NotImplementedException();
    }

    public virtual void ReactToExplosion(float damage)
    {
        Hurt(damage);
    }
}
