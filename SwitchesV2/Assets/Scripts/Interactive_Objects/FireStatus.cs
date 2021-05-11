using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStatus : MonoBehaviour
{
    public ParticleSystem ps;
    public Enemy en;
    
   // private ReactiveBox rb;

    public float fireDamage = 3f;
    public float cooldown;
    private float nextTimeToStop;

    private float nextTimeHurt;

    public float hurtCooldown;
    
    
    // Start is called before the first frame update
    void Start()
    {
       // rb = GetComponentInParent<ReactiveBox>();
       // ps.Stop();
        

        
    }

    private void Awake()
    {
        ps.Stop();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
       
        
        
            if (Time.time < nextTimeToStop )
            {
                 
               // en.Hurt( fireDamage*Time.deltaTime);
              //-- fireDamage*Time.deltaTime
              if (Time.time > nextTimeHurt)
              {
                  en.Hurt(fireDamage);
                  nextTimeHurt = Time.time + hurtCooldown;
              }
              
              
                
            }
                

            else
            {
                ps.Stop();
                enabled = false;
            }
        
        
    }

    private void OnEnable()
    {
        Debug.Log("ciao");
        ps.Play();
        nextTimeToStop = Time.time + cooldown;
        nextTimeHurt = Time.time + hurtCooldown;
    }

    

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        ReactiveObject ro = go.GetComponentInParent<ReactiveObject>();
        if(ro!=null && enabled)
            go.GetComponentInParent<ReactiveObject>().reactToFire(fireDamage);
    }

    public void restartCooldown()
    {
        nextTimeToStop = Time.time + cooldown;
    }
}
