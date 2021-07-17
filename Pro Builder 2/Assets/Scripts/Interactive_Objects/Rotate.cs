using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 1000f;

    public ReactiveFan rf;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
            transform.Rotate(0,0,Time.deltaTime * speed,Space.Self);
        if(!rf.isInBox && speed>0)
        {
            speed = speed - 0.5f;
        }
        
        //rb.AddTorque(transform.forward, ForceMode.Acceleration);
    }
}
