using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Control Script/Reactive Box")]

public class ReactiveBox  : MonoBehaviour, ReactiveObject
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void ReactToAttraction(Vector3 target, float attractionSpeed)
    {
        rb.freezeRotation = true;
        rb.useGravity = false;
        if (rb.position.Equals(target)) return;
        else rb.position = Vector3.MoveTowards(rb.position, target, attractionSpeed);
    }

    public void ReactToRepulsing(Vector3 direction, float repulsingSpeed)
    {
        rb.AddForce(direction * repulsingSpeed, ForceMode.Impulse);
    }

    public void ReactToReleasing()
    {
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.useGravity = true;
    }

    public void ReactToLaunching(Vector3 direction, float launchingSpeed)
    {
        rb.AddForce(direction * launchingSpeed, ForceMode.Impulse);
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.useGravity = true;
    }

    
}
