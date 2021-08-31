using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    // Start is called before the first frame update
    public int numberOfChildrens;
    public bool canEnable = false;
    public bool transition = false;
    public GameObject interactObj;
    private InteractableObject objToInteract;
    public bool[] combination;
    //private bool[] childrenActivation;
    public Dummy[] dummies;
    
    public 
    void Start()
    {
        objToInteract= interactObj.GetComponent<InteractableObject>();
    }

    /*
     * switch controller enables the interactive object only if the inner combination
     * is equal to all switch combination. This system forms a binary code that must be
     * setted in order to enable that object. If you have only one dummy to check this is
     * the case of only on-off simple mechanism.
     */
    // Update is called once per frame
    void Update()
    {
        //optimization: check the combination only if there's a variation in switch positions.
        if (transition)
        {
            for (int i = 0; i < numberOfChildrens; i++)
            {
                if (dummies[i].getEnabled() != combination[i])
                {
                    canEnable = false;
                    break;
                }
                
                if (i == numberOfChildrens - 1)
                {
                    canEnable = true;
                }


            }

            if (canEnable)
                objToInteract.setTrue();

            else
            {
                objToInteract.setFalse();
            }

            transition = false;

        }
    }
}
