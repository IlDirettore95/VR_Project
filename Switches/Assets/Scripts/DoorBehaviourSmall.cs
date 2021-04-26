using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviourSmall : MonoBehaviour
{
    
    public GameObject door;
    
    private bool hasCollided = false;
    private bool hasGone = false;
    private float maxOpening = 2f;
    private float maxClosing = 2f;
    private float TotalOpening = 0f;
    private float TotalClosing = 0f;
    private float opening = 0;
    private float closing = 0;
    private float speed = 2f;
    
    
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
                door.transform.Translate(-opening, 0, 0, Space.Self);
                
            }
            
            
            TotalClosing = maxClosing- TotalOpening;
            


        }
        
        else if(hasGone)
        {
                
            if (TotalClosing < maxClosing)
            {
                closing = Time.deltaTime * speed;
                TotalClosing += closing;
                door.transform.Translate(closing,0,0,Space.Self);
                
            }
                
                
                
            TotalOpening = maxOpening - TotalClosing;
                

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl!=null && pl.tag.Equals("Player"))
        {
            hasCollided = true;
            hasGone = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl!=null && pl.tag.Equals("Player"))
        {
            hasGone = true;
            hasCollided = false;
        }
    }
}
