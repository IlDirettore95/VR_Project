using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour, ReactiveObject
{

    //general 
    protected bool attracted = false;
    protected bool isAlive = true;

    //Enemy manager
    protected EnemiesManager enemyManager;
    public int areaID;
    public int enemyID;

    //Enemy status
    public float MaxHealth;
    protected float _health;
    
    //Speed
    public float walkingSpeed;
    public float triggeredSpeed;
    public float walkingSpeedBuildUp;
    public float triggeredSpeedBuildUp;
    protected float currentStateBuildUp;

    //Movement
    protected float speed;  //Used to calculate damage when the enemy is launched by the player against static objects _agent.speed = speed;

    //Collision damage
    public float impactVelocityThreashold;

    //Trigger
    public float triggerPlayerDistance;
    public float triggerEnemyDistance; //Max distance in which the enemy will trigger other enemies of the same area

    //Attack
    public float attackDistance;

    //List of destination for patrolling
    //After a cooldown once the enemy has reached hos destination it will change it;
    public Transform[] patrollingPoints;
    private int pointIndex = -1; //every time an enemy wants patrolling this index is incremented, so the first time it will be set to 0
    public float patrollingDistanceThreashold;
    public float guardingCooldown;
    protected float nextTimePatrol;

    //AI
    protected NavMeshAgent _agent;
    protected Transform playerTransform;
    protected Rigidbody rb;
    protected PlayerStatus _playerStatus;

    //Player Interaction
    protected Transform target;

    //Is attracted now?
    public bool IsAttracted() => attracted;

    //Getters and setter for the area ID;
    public void SetAreaID(int ID) => areaID = ID;

    public int GetAreaID() => areaID;

    public void SetID(int id) => enemyID = id;

    public int GetID() => enemyID;

    

    public virtual void Hurt(float damage) //Take damage and decrease enemy healt
    {
        if (damage >= 1 && isAlive)
        {
            _health -= damage;
            if (_health <= 0) _health = 0;
            if (_health == 0) isAlive = false;
        }  
    }

    protected void Die()
    {
        Debug.Log("Nemico morto");
        enemyManager.CollectEnemy(gameObject);
    }
   
    public virtual void Revive()
    {
        attracted = false;

        _health = MaxHealth;
        rb.useGravity = false;
        rb.isKinematic = true;
        _agent.enabled = true;
    }

    //This method handles enemy's patrolling between patrolling points
    protected virtual void Patrol()
    {
        _agent.destination = patrollingPoints[pointIndex].position;
    }

    protected void ChangeDestination()
    {
        pointIndex = (pointIndex + 1) % patrollingPoints.Length;
    }

    protected bool isAtDestination()
    {
        return (Vector3.Distance(transform.position, patrollingPoints[pointIndex].position) <= patrollingDistanceThreashold);
    }

    public virtual void ReactToAttraction(float attractionSpeed)
    {
        _agent.enabled = false;
        rb.isKinematic = false;
        attracted = true;
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);
    }

    public virtual void ReactToReleasing()
    {
        attracted = false;
        rb.useGravity = true;
        rb.freezeRotation = false;
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
    }

    public virtual void ReactToLaunching(float launchingSpeed)
    {
        attracted = false;
        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);
    }

    public virtual void ReactToExplosion(float damage, float power, Vector3 center, float radius)
    {
        Hurt(damage);
        rb.AddExplosionForce(power, center, radius, 0.2f, ForceMode.Impulse);
    }

    public virtual void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public virtual bool IsDestroyed() => !isAlive;

}
