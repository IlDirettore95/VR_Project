using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private bool onoff = false;
    public GameObject interactObj;
    private InteractableObject objToInteract;
    private SwitchController sc;
    public Light offLight;
    public Light onLight;
    // Start is called before the first frame update
    void Start()
    {
        offLight.enabled = true;
        onLight.enabled = false;
        //passing from gameObject to a generic interactableObject defined by the interface
       objToInteract= interactObj.GetComponent<InteractableObject>();

       sc = GetComponentInParent<SwitchController>();
    }

  

  

    public void commutation()
    {
        //comunicate to SwitchController that there's a commutation -> enable checking.
        sc.transition = true;
        //commutation on->off
        if (onoff)
        {
            onoff = false;
            objToInteract.setFalse();
            offLight.enabled = true;
            onLight.enabled = false;
            Debug.Log("ho disattivato l'interruttore");
            
        }
        else
            //commutation off->on
        {
            onoff = true;
            objToInteract.setTrue();
            offLight.enabled = false;
            onLight.enabled = true;
            Debug.Log("ho attivato l'interruttore");
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
