using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveFan : MonoBehaviour, ReactiveObject
{
    private Rigidbody rb;
    private GameObject player;
    private Transform target;
    private bool isInBox = true;
    private bool isOnGround = false;
    public float damage;
    public float _health;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        target = GameObject.Find("ObjectGrabber").transform;
        rb.maxAngularVelocity = 20f;
    }

    private void Update()
    {
        if(isInBox)
        rb.AddTorque(transform.forward, ForceMode.Acceleration);
    }

    public void ReactToRepulsing(Vector3 direction, float repulsingSpeed)
    {
        rb.AddForce(direction * repulsingSpeed, ForceMode.Impulse);
    }

   

  

    public void ReactToAttraction(float attractionSpeed)
    {
        isInBox = false;
        rb.useGravity = false;
        rb.freezeRotation = false;

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

        rb.freezeRotation = false;
        rb.useGravity = true;
        isOnGround = true;
        
    }

   

    public void ReactToLaunching(float launchingSpeed)
    {
        
        rb.freezeRotation = false;
        rb.useGravity = true;
        //Add a torque to add randomness to movements
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);
        isOnGround = true;
    }

    public bool IsDestroyed()
    {
        return _health == 0;
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
