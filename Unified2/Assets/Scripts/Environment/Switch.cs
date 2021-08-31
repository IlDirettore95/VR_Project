using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    //can set on or off by default in the scene.
    //if the switch is on, the obj to interact will
    //start already enabled, disabled viceversa.
    public bool onoff;
    public GameObject interactObj;
    private InteractableObject objToInteract;
    private SwitchController sc;
    
    [SerializeField] private Material enabled_material;
    [SerializeField] private Material disabled_material;

   
    
   

    private bool switchEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        //checking if the switch starts enabled or disabled, switch light must change.
        if (onoff)
        {
            

            GetComponent<MeshRenderer>().material = enabled_material;

        }
        else
        {
           
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
            //this mechanism is an optimization, check a combination only if there's a commutation.
            sc.transition = true;
            //commutation on->off
            if (onoff)
            {
                onoff = false;
                objToInteract.setFalse();
                GetComponent<MeshRenderer>().material = disabled_material;

            }
            else
                //commutation off->on
            {
                onoff = true;
                objToInteract.setTrue();
                GetComponent<MeshRenderer>().material = enabled_material;
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
