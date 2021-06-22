using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class ReactiveBox  : MonoBehaviour, ReactiveObject
{
    private Rigidbody rb;
    private GameObject player;
    private Transform target;
    private FireStatus fs;
   
    public float _health;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        target = GameObject.Find("ObjectGrabber").transform;

        fs = GetComponent<FireStatus>();
    }

    
    public void ReactToAttraction(float attractionSpeed)
    {
        if (GetComponentInParent<PlatformBehaviour>() != null)
        {
            
            transform.SetParent(null);
        }
        
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);
    }

    public void ReactToReleasing()
    {
        rb.useGravity = true;
        rb.freezeRotation = false;
    }

  

    public void ReactToLaunching(float launchingSpeed)
    {
        
        rb.freezeRotation = false;
        rb.useGravity = true;
        //Add a torque to add randomness to movements
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);
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
        return IsAttracted();
    }

    public void reactToExplosion(float damage)
    {
        throw new NotImplementedException();
    }

    public void reactToFan(Vector3 direction, float angularVelocity, float damage, bool isInBox)
    {
        if(isInBox)
            rb.AddForce(direction * angularVelocity , ForceMode.Impulse);
    }
}
