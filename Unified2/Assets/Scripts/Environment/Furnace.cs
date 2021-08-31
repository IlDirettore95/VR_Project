using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour, InteractableObject
{
    public float fireDamage;

    public bool isEnabled;

    public GameObject fire;

    private ParticleSystem[] fires;

    private Light light;

    private AudioSource _audioSource;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        fires =  fire.GetComponentsInChildren<ParticleSystem>();

        light = fire.GetComponentInChildren<Light>();

        //disable particle system if fire is not enabled, enable viceversa.
        if (!isEnabled)
        {
            foreach (var ps in fires)
            {
                ps.Stop();
            }
            light.enabled = false;
            _audioSource.Stop();
        }
        else
        {
            _audioSource.Play();
        }
        
        
    }
    //damage the player while he stay in the fire collider or react to fire for ReactiveObject
    public void OnTriggerStay(Collider other)
    {
        
            GameObject gameObject = other.gameObject;
            if (gameObject.GetComponent<PlayerStatus>()!=null && isEnabled)
            {
                PlayerStatus pl = gameObject.GetComponent<PlayerStatus>();
                pl.Hurt(fireDamage * Time.deltaTime);

            }
            else if (gameObject.GetComponentInParent<ReactiveObject>() != null && isEnabled)
            {
                ReactiveObject en = gameObject.GetComponentInParent<ReactiveObject>();
                en.reactToFire(fireDamage);
            } 
    }

    public void setTrue()
    {
        isEnabled = true;
      
       foreach (var ps in fires)
       {
           ps.Play();
       }
        light.enabled = true;
        _audioSource.Play();
    }

    public void setFalse()
    {
        isEnabled = false;
        foreach (var ps in fires)
        {
           ps.Stop();
        }
        light.enabled = false;
        _audioSource.Stop();
    }
}


