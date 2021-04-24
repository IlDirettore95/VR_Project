using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour, ReactiveObject
{
    //Attracted
    protected bool attracted = false;

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

    //Handles collision damage
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody colliderRb = collision.gameObject.GetComponent<Rigidbody>();
        if(colliderRb != null)
        {
            if (collision.relativeVelocity.magnitude > impactVelocityThreashold)
            {
                _agent.enabled = false;
                rb.isKinematic = false;
                rb.useGravity = true;
                Hurt(colliderRb.mass * collision.relativeVelocity.magnitude);
                //L'enemy si muoverà?
            }
        }
        //The collided object is assumed to be static
        else if (speed > impactVelocityThreashold) Hurt(rb.mass * speed);
            
    }

    public virtual void Hurt(float damage) //Take damage and decrease enemy healt
    {
        if (damage >= 1)
        {
            _health -= damage;
            if (_health <= 0) _health = 0;
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

    public virtual void MoveTo(Vector3 point)//Move the enemy from the current position to point position in input
    {
        throw new System.NotImplementedException();
    }

    public virtual void Patrol(Vector3[] path)//Patrol the path described by the array of position in input
    {
        throw new System.NotImplementedException();
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

    public virtual void ReactToExplosion(float damage)
    {
        Hurt(damage);
    }

    public virtual void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public virtual bool IsDestroyed()
    {
        return false;
    }

}
