using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour, InteractableObject
{
    public GameObject leftDoor;

    public GameObject rightDoor;

    public Light pointlight1;
    public Light pointlight2;
    public Light pointlight3;
    public Light spotLight;
    public GameObject neon;

    private bool hasCollided = false;
    private bool hasGone = false;
    private bool isEnabled = false;
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
        if (pl!=null && pl.tag.Equals("Player") && isEnabled)
        {
            hasCollided = true;
            hasGone = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl!=null && pl.tag.Equals("Player") && isEnabled)
        {
            hasGone = true;
            hasCollided = false;
        }
    }
    
    
    public void setTrue()
    {
        neon.GetComponent<Renderer>().material.color = new Color(0, 1, 0,0.5f);
        neon.GetComponent<Renderer>().material.SetColor("_EMISSION",new Color(0, 1, 0,0.5f) );
        
        pointlight1.color = new Color(0, 1, 0);
        pointlight2.color =  new Color(0, 1, 0);
        pointlight3.color =  new Color(0, 1, 0);
        spotLight.color =  new Color(0, 1, 0);
        
        
        
        Debug.Log("abilito");
        isEnabled = true;
    }

    public void setFalse()
    {
        neon.GetComponent<Renderer>().material.color = new Color(1, 0, 0,0.5f);
        neon.GetComponent<Renderer>().material.SetColor("_EMISSION",new Color(1, 0, 0,0.5f) );
        
        pointlight1.color = new Color(1, 0, 0);
        pointlight2.color = new Color(1, 0, 0);
        pointlight3.color = new Color(1, 0, 0);
        spotLight.color = new Color(1, 0, 0);
        Debug.Log("disabilito");
        isEnabled = false;
    }
}
