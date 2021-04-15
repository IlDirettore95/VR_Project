﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input with Jump")]
public class FPSInput_Jump : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = -9.8f;
    public float mass = 1f;
    public float jumpSpeed = 9.0f;
    public float fuel = 100f;
    public int maxFuel = 100;
    public int minFuel = 0;
    public int dischargingRate = 20;
    public int chargingRate = 40;
    private bool jetpack = false;

    private CharacterController _charController;
    private float _deltaY = 0f; //Remember your Y velocity e.g. during a jump
    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
       
        Debug.Log("fuel: " +fuel);
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        //Vertical movement (jumping)
        if (_charController.isGrounded )
        {
            
            jetpack = false;
            if (fuel < maxFuel)
            {
                fuel += chargingRate * Time.deltaTime;
                if (fuel > maxFuel){ fuel = maxFuel;}
            }
            

            if (Input.GetButtonDown("Jump"))
            {
                _deltaY = jumpSpeed;
                
            }
        }
        
        else
        {
            if (Input.GetButton("Jump") && jetpack && fuel > 0 )
            {
                fuel -= dischargingRate *  Time.deltaTime;
                if (fuel < minFuel) fuel = minFuel;
                _deltaY = jumpSpeed;
            }
            else if (Input.GetButtonDown("Jump") && !jetpack)
            {
                _deltaY = jumpSpeed;
                jetpack = true;
            }
                
            
            _deltaY += gravity * mass * Time.deltaTime;
            
        }
       
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = _deltaY;
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
    }
}