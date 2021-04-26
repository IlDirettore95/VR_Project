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

    public float _health;

    private bool isDestroyed = false;
    private bool isAttracted = false;

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


    public void ReactToAttraction(float attractionSpeed)
    {
        isAttracted = true;
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.velocity = (target.position - rb.position).normalized * attractionSpeed * Vector3.Distance(target.position, rb.position);
    }

    public void ReactToReleasing()
    {
        isAttracted = false;
        rb.useGravity = true;
        rb.freezeRotation = false;
    }


    public void ReactToLaunching(float launchingSpeed)
    {
        isAttracted = false;
        rb.freezeRotation = false;
        rb.useGravity = true;
        //Add a torque to add randomness to movements
        rb.AddTorque(0.05f, 0.05f, 0.05f, ForceMode.Impulse);
        rb.AddForce(target.forward * launchingSpeed, ForceMode.Impulse);
    }

    public void ReactToExplosion(float damage, float power, Vector3 center, float radius)
    {
        rb.AddExplosionForce(power, center, radius, 0.2f, ForceMode.Impulse);
    }

    public bool IsDestroyed() => isDestroyed;

    public bool IsAttracted() => isAttracted;
}
