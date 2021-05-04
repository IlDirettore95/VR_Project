using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStatus : MonoBehaviour
{
    public ParticleSystem ps;
    public float ticks;
    private float tmp;
    private bool isOnFire = false;
   // private ReactiveBox rb;

    public float fireDamage = 3f;
    public float cooldown;
    private float nextTimeToStop;
    
    // Start is called before the first frame update
    void Start()
    {
       // rb = GetComponentInParent<ReactiveBox>();
        ps.Stop();
        tmp = ticks;

        
    }

    // Update is called once per frame
    void Update()
    {
       
        if (isOnFire)
        {
            if (Time.time < nextTimeToStop )
            {
              //  rb._health--;
              //-- fireDamage*Time.deltaTime
                
            }
                

            else
            {
                ps.Stop();
                isOnFire = false;
            }
        }
        
    }

    public void onFire()
    {
        if (!isOnFire)
        {
            ticks = tmp;
            ps.Play();
            isOnFire = true;
            nextTimeToStop = Time.time + cooldown;
        }
        else
        {
            nextTimeToStop = Time.time + cooldown;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if(go.GetComponent<ReactiveObject>()!= null && isOnFire)
            go.GetComponent<ReactiveObject>().reactToFire(fireDamage);
    }
    
}
