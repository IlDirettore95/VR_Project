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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        target = GameObject.Find("ObjectGrabber").transform;
    }

    public void ReactToRepulsing(Vector3 direction, float repulsingSpeed)
    {
        rb.AddForce(direction * repulsingSpeed, ForceMode.Impulse);
    }

    public void ReactToIncreasing()
    {
        throw new NotImplementedException();
    }

    public void ReactToDecreasing()
    {
        throw new NotImplementedException();
    }

    public void ReactToAttraction(float attractionSpeed)
    {
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);
    }

    public void ReactToReleasing()
    {
        rb.useGravity = true;
        rb.freezeRotation = false;
    }

    public void ReactToRepulsing()
    {
        throw new NotImplementedException();
    }

    public void ReactToLaunching(float launchingSpeed)
    {
        
        rb.freezeRotation = false;
        rb.useGravity = true;
        //Add a torque to add randomness to movements
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);
    }
}
