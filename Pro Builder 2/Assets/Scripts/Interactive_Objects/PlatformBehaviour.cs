using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    public float maxElevation = 10f;

    public float maxDescent = 10f;

    private float currentElevation = 0f;

    private float currentDescent = 0f;

    private float instantElevation = 0f;

    private float instantDescent = 0f;

    private bool onEnter = false;

    private bool onExit = false;

    public float speed = 1f;

    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (onEnter)
        {

            if (currentElevation < maxElevation)
            {
                instantElevation = Time.deltaTime * speed;
                currentElevation += instantElevation;
                transform.Translate(0,instantElevation,0,Space.Self);
               
            }

            currentDescent = maxDescent - currentElevation;

        }
        
        else if (onExit)
        {
            if (currentDescent < maxDescent)
            {
                instantDescent = Time.deltaTime * speed;
                currentDescent += instantDescent;
                transform.Translate(0,-instantDescent,0,Space.Self);
                

            }

            currentElevation = maxElevation - currentDescent;
            
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision");
        GameObject player = other.gameObject;
        if (player != null && player.tag.Equals("Player"))
        {
            Debug.Log("ciao");
            player.transform.SetParent(transform);
            onEnter = true;
            onExit = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject player = other.gameObject;
        if (player != null && player.tag.Equals("Player"))
        {
            player.transform.SetParent(null);
            onExit = true;
            onEnter = false;
        }
    }
}
