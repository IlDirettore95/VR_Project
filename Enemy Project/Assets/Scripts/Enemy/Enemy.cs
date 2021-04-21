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
    protected bool dead = false;
    protected bool attracted = false;
    protected bool throwed = false;

    //Enemy status
    public float MaxHealth;
    protected float _health;
    public float damageThreashold;

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

    public int GetAreaID()
    {
        return areaID;
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
                throwed = true;
                _agent.enabled = false;
                rb.isKinematic = false;
                rb.useGravity = true;
                triggered = true;
                float damage = colliderRb.mass * colliderRb.velocity.magnitude;
                Debug.Log("Damage = " + damage);
                Hurt(damage);
            }
        }
        else
        {
            if(rb.velocity.magnitude > damageThreashold)
            {
                float damage = rb.mass * speed;
                //damage = Mathf.Clamp(damage, 1, )
                Debug.Log("Damage = " + damage);
                Hurt(damage);
            }
        }
    }

    public void Hurt(float damage)
    {
        if (!dead && damage >= 1)
        {
            triggered = true;
            _health -= damage;
            if (_health <= 0) dead = true;
        }  
    }

    protected void Die()
    {
        Debug.Log("Nemico morto");
        enemyManager.CollectEnemy(gameObject);
    }
   
    public virtual void Revive()
    {
        idle = true;
        walking = false;
        triggered = false;
        throwed = false;
        attracted = false;
        dead = false;

        _health = MaxHealth;
        rb.useGravity = false;
        rb.isKinematic = true;
        _agent.enabled = true;
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
        triggered = true;
        _agent.enabled = false;
        rb.isKinematic = false;
        attracted = true;
        throwed = false;
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
        attracted = false;
        throwed = true;
        rb.useGravity = true;
        rb.freezeRotation = false;
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
    }

    public virtual void ReactToLaunching(float launchingSpeed)
    {
        attracted = false;
        throwed = true;
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

    public virtual void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public bool IsDestroyed()
    {
        return false;
    }
}
