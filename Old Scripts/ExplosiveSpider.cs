using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class ExplosiveSpider : MonoBehaviour, IEnemy, ReactiveObject
{


    private bool idle = true;
    private bool walking = false;
    private bool triggered = false;
    private bool exploding = false;
    private bool dead = false;

    //Enemy status
    public float MaxHealth; //@TODO
    private float _health; //@TODO

    //Speed
    public float walkingSpeed; //@TODO
    public float triggeredSpeed; //@TODO
    public float walkingSpeedBuildUp; //@TODO
    public float triggeredSpeedBuildUp; //@TODO
    private float speed; //General speed

    //Explosion
    public float explosionDamage;
    public float explosionCooldown;
    private float nextTimeExplosion;
    public float explosionRadius;
    public float attackDistance;

    //Trigger
    public float triggerPlayerDistance;
    public float triggerEnemyDistance;  //@TODO
    public Transform spawnPoint; //@TODO

    //AI
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private Transform _player;
    private PlayerStatus _playerStatus;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _player = GameObject.Find("Player").transform;
        _playerStatus = _player.GetComponent<PlayerStatus>();
        _health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(idle)
        {
            //Forse cammino

            //Lerping speed to walking speed
            speed = Mathf.Lerp(speed, walkingSpeed, walkingSpeedBuildUp * Time.deltaTime);
            walking = true;
            idle = false;
        }
        else if(walking)
        {
            //Lerping speed to walking speed
            triggered = true;
            walking = false;
            speed = Mathf.Lerp(speed, walkingSpeed, walkingSpeedBuildUp * Time.deltaTime);
            
            //Finding the player
            _agent.destination = _player.position;
            //transform.LookAt(_player.position);
        }
        else if (triggered)
        {
            //Is no more calm
            //Debug.Log("TRIGGERED");

            //Lerping speed to triggered speed
            speed = Mathf.Lerp(speed, triggeredSpeed, triggeredSpeedBuildUp * Time.deltaTime);

            //Finding the player
            _agent.destination = _player.position;
            //transform.LookAt(_player.position);

            //Follow the player
            if(_agent.remainingDistance <= attackDistance)
            {
                Debug.Log("EXPLODE");
                exploding = true;
                triggered = false;
                nextTimeExplosion = Time.time + explosionCooldown;
            }
        }
        else if (exploding)
        {
            //Desabling NavMesh
            _agent.isStopped = true;
            if(Time.time >= nextTimeExplosion)
            {
                Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRadius);
                for(int i = 0; i < collisions.Length; i++)
                {
                    if(collisions[i].gameObject.GetComponent<PlayerStatus>() != null)
                    {
                        //The explosion has hit the player
                        _playerStatus.Hurt(explosionDamage);
                        Debug.Log("Colpito! Ora hai solo " + _playerStatus.GetHealth() + " di vita.");
                    }
                    else if(collisions[i].gameObject.GetComponent<ReactiveEnemy>() != null)
                    {
                        collisions[i].gameObject.GetComponent<ReactiveEnemy>().ReactToExplosion(explosionDamage);
                    }
                }
                Destroy(this.gameObject);
            }
            
        }

        //passa da idle a walking
        // ogni tanto a caso

        //passa da idle / walking a trigger
        //il player è  abbastanza vicino o un nemico vicino è triggerato

        //passa da triggered a idle o walking
        //Se il player è morto, se si allontana troppo da un certo punto.

        //Explosion
        //la sua vita va a 0, oppure è rimasto per un certo tempo vicino al giocatore

        //Dead
        //è esploso;
        if(_health == 0)
        {
            Explode();
        }


    }

    private void Explode()
    {
        exploding = true;
        idle = false;
        walking = false;
        triggered = false;
    }

    public void ReactToExplosion(float damage)
    {
        Explode();
        Debug.Log("Sono esploso per colpa di un Ragno!");
    }

    public void Hurt(float damage)
    {
        _health -= damage;
        if (_health < 0) _health = 0;
    }

    public void Revive()
    {
        throw new System.NotImplementedException();
    }

    public void BeTriggered()
    {
        throw new System.NotImplementedException();
    }

    public void MoveTo(Vector3 point)
    {
        throw new System.NotImplementedException();
    }

    public void Patrol(Vector3[] path)
    {
        throw new System.NotImplementedException();
    }

    public void ReactToAttraction(float attractionSpeed)
    {
        throw new System.NotImplementedException();
    }

    public void ReactToRepulsing()
    {
        throw new System.NotImplementedException();
    }

    public void ReactToReleasing()
    {
        throw new System.NotImplementedException();
    }

    public void ReactToLaunching(float launchingSpeed)
    {
        throw new System.NotImplementedException();
    }

    public void ReactToIncreasing()
    {
        throw new System.NotImplementedException();
    }

    public void ReactToDecreasing()
    {
        throw new System.NotImplementedException();
    }
}
