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

    private AudioSource _audioSource;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        fires =  fire.GetComponentsInChildren<ParticleSystem>();

        if (!isEnabled)
        {
            foreach (var ps in fires)
            {
                ps.Stop();
            }
            
            _audioSource.Stop();
        }
        else
        {
            
            _audioSource.Play();
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
       
       _audioSource.Play();
    }

    public void setFalse()
    {
        Debug.Log("disabilito");
        isEnabled = false;
        foreach (var ps in fires)
        {
           ps.Stop();
        }
        
        _audioSource.Stop();
    }
}


