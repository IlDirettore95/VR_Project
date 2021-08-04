using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, InteractableObject
{
    private bool isEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setTrue()
    {
        isEnabled = true;
    }

    public void setFalse()
    {
        isEnabled = false;
    }

    public bool getEnabled()
    {
        return isEnabled;
    }
}
