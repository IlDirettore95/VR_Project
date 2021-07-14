using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;

public class ReactiveFan : MonoBehaviour, ReactiveObject
{
    private Rigidbody rb;
    private GameObject player;
    private Transform target;
    public bool isInBox = true;
    private bool isOnGround = false;
    public float damage;
    public float _health;
    public float buildUp;
    private float buildUpState = 0;
    private Quaternion initialRotation;
    private Quaternion finalRotation;
    private bool isAttracted = false;
    private FireStatus fs;
    
    public Vector3 targetAngle = new Vector3(90f, 0f, 0f);
 
    private Vector3 currentAngle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        target = GameObject.Find("ObjectGrabber").transform;
        rb.maxAngularVelocity = 20f;

        finalRotation = new Quaternion(0.5f, -0.5f, 0.5f, 0.5f);
        fs = GetComponentInChildren<FireStatus>();
        
        currentAngle = transform.eulerAngles;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //if(isInBox)
          //  rb.AddTorque(transform.forward, ForceMode.Acceleration);
    }

    public void ReactToRepulsing(Vector3 direction, float repulsingSpeed)
    {
        rb.AddForce(direction * repulsingSpeed, ForceMode.Impulse);
    }

   

  

    public void ReactToAttraction(float attractionSpeed)
    {
       // if (!isAttracted)
        //{

            isInBox = false;
            rb.useGravity = false;
            rb.freezeRotation = false;



            //initialRotation = transform.rotation;
            isAttracted = true;
            
            currentAngle = new Vector3(
                Mathf.LerpAngle(currentAngle.x, targetAngle.x, Time.deltaTime),
                transform.eulerAngles.y,
               transform.eulerAngles.z);
 
            transform.eulerAngles = currentAngle;
            

       // }
/*
        if (buildUpState <1)
        {
            buildUpState += buildUp* Time.deltaTime;
            Quaternion temp = Quaternion.Lerp(initialRotation, finalRotation, buildUpState);
        
            rb.MoveRotation(Quaternion.Euler(temp.eulerAngles.x,temp.eulerAngles.y,transform.rotation.eulerAngles.z));
        }
       
       */
        
      

        if (isOnGround)//se raccolgo la ventola dopo averla staccata allora disabilito le rotazioni tranne sulla z
        {
            rb.freezeRotation = true;
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
        }
        
        rb.constraints &= ~RigidbodyConstraints.FreezePosition;
       
        
        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);
        
        
    }

 

    public void ReactToReleasing()
    {
        buildUpState = 0;
        isAttracted = false;
        rb.freezeRotation = false;
        rb.useGravity = true;
        isOnGround = true;
        
    }

   

    public void ReactToLaunching(float launchingSpeed)
    {
        buildUpState = 0;
        isAttracted = false;
        
        rb.freezeRotation = false;
        rb.useGravity = true;
        //Add a torque to add randomness to movements
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);
        isOnGround = true;
    }

    public void reactToFire(float damage)
    {
        if(!fs.enabled)
            fs.enabled = true;

        else
        {
            fs.restartCooldown();
        }
    }

    public bool IsDestroyed()
    {
        return _health == 0;
    }

    public bool IsAttracted()
    {
        return isAttracted;
    }

    public void reactToExplosion(float damage)
    {
        throw new System.NotImplementedException();
    }

    public void reactToFan(Vector3 direction, float angularVelocity, float damage, bool isInBox)
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionEnter(Collision other)
    {
        GameObject go = other.gameObject;
        if (go.GetComponent<Rigidbody>() != null)
        {
            if (go.GetComponent<ReactiveObject>() != null)
                go.GetComponent<ReactiveObject>().reactToFan(transform.forward, rb.angularVelocity.magnitude, damage, isInBox);
              
                
        }
    }
}
