using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour, InteractableObject
{
    public float fireDamage;

    private bool isEnabled = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
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
            else if (gameObject.GetComponent<ReactiveEnemy>()!=null && isEnabled)
            {
                ReactiveEnemy en = gameObject.GetComponent<ReactiveEnemy>();
                en.reactToFire(fireDamage);
            }
        
        
    }

    public void setTrue()
    {
        Debug.Log("abilito");
        isEnabled = true;
    }

    public void setFalse()
    {
        Debug.Log("disabilito");
        isEnabled = false;
    }
}


