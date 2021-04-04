using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    public GameObject leftDoor;

    public GameObject rightDoor;
    
    

    private bool hasCollided = false;
    private bool hasGone = false;
    private float maxOpening = 1.25f;
    private float maxClosing = 1.25f;
    private float TotalOpening = 0f;
    private float TotalClosing = 0f;
    private float opening = 0;
    private float closing = 0;
    private float speed = 1f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasCollided)
        {
            
            if (TotalOpening < maxOpening)
            {
                opening = Time.deltaTime * speed;
                TotalOpening += opening;
                leftDoor.transform.Translate(-opening, 0, 0, Space.Self);
                rightDoor.transform.Translate(-opening, 0, 0, Space.Self);
            }
            
            
                TotalClosing = maxClosing- TotalOpening;
            


        }
        
        else if(hasGone)
            {
                
                if (TotalClosing < maxClosing)
                {
                    closing = Time.deltaTime * speed;
                    TotalClosing += closing;
                    leftDoor.transform.Translate(closing,0,0,Space.Self);
                    rightDoor.transform.Translate(closing,0,0,Space.Self);
                }
                
                
                
                    TotalOpening = maxOpening - TotalClosing;
                

            }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl!=null && pl.tag.Equals("PLayer"))
        {
            hasCollided = true;
            hasGone = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl!=null && pl.tag.Equals("PLayer"))
        {
            hasGone = true;
            hasCollided = false;
        }
    }
}
