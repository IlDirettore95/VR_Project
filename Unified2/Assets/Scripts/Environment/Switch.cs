using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public bool onoff;
    public GameObject interactObj;
    private InteractableObject objToInteract;
    private SwitchController sc;
    
    [SerializeField] private Material enabled_material;
    [SerializeField] private Material disabled_material;

   
    
   // public Light offLight;
  // public Light onLight;

    private bool switchEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        
        
        
        if (onoff)
        {
            //offLight.enabled = false;
            //onLight.enabled = true;

            GetComponent<MeshRenderer>().material = enabled_material;

        }
        else
        {
           // offLight.enabled = true;
            //onLight.enabled = false;
            GetComponent<MeshRenderer>().material = disabled_material;
        }
        
        //passing from gameObject to a generic interactableObject defined by the interface
       objToInteract= interactObj.GetComponent<InteractableObject>();

       sc = GetComponentInParent<SwitchController>();
    }

  

  

    public void commutation()
    {
        if (switchEnabled)
        {
            //comunicate to SwitchController that there's a commutation -> enable checking.
            sc.transition = true;
            //commutation on->off
            if (onoff)
            {
                onoff = false;
                objToInteract.setFalse();
                //offLight.enabled = true;
                //onLight.enabled = false;
                GetComponent<MeshRenderer>().material = disabled_material;
                Debug.Log("ho disattivato l'interruttore");

            }
            else
                //commutation off->on
            {
                onoff = true;
                objToInteract.setTrue();
                //offLight.enabled = false;
                //onLight.enabled = true;
                GetComponent<MeshRenderer>().material = enabled_material;
                Debug.Log("ho attivato l'interruttore");
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void enableSwitch()
    {
        switchEnabled = true;
    }

    public void disableSwitch()
    {
        switchEnabled = false;
    }

    public void commutationDelayed(float delay)
    {
        StartCoroutine(changeColor(delay));
    }

    private IEnumerator changeColor(float delay)
    {
        yield return new WaitForSeconds(delay);
        commutation();
    }
}
