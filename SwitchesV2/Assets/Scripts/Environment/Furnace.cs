using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour, InteractableObject
{
    public float fireDamage;

    private bool isEnabled = false;

    public GameObject fire;

    private ParticleSystem[] fires;
    
    // Start is called before the first frame update
    void Start()
    {
        fires =  fire.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in fires)
        {
            ps.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider other)
    {
        
            GameObject gameObject = other.gameObject;
            if (gameObject.GetComponent<PlayerStatus>()!=null && isEnabled)
            {
                PlayerStatus pl = gameObject.GetComponent<PlayerStatus>();
                pl.Hurt(fireDamage * Time.deltaTime);
                Debug.Log("faccio danno");

            }
            else if (gameObject.GetComponentInParent<ReactiveObject>()!=null && isEnabled)
            {
                Debug.Log("fire!");
                ReactiveObject en = gameObject.GetComponentInParent<ReactiveObject>();
                en.reactToFire(fireDamage);
            }
            
            
        
        
    }

    public void setTrue()
    {
        Debug.Log("abilito");
        isEnabled = true;
      
       foreach (var ps in fires)
       {
           ps.Play();
       }
    }

    public void setFalse()
    {
        Debug.Log("disabilito");
        isEnabled = false;
        foreach (var ps in fires)
        {
           ps.Stop();
        }
    }
}


