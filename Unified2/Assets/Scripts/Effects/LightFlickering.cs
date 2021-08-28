using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickering : MonoBehaviour
{
    [SerializeField] private Light light;
    public float flickeringWait;
    private float nextTimeFlickering;
    public bool _enableLightOnDisable;
   
    void OnEnable()
    {
        nextTimeFlickering = Time.time + flickeringWait;
    }

    
    private void OnDisable()
    {
        if(_enableLightOnDisable) light.enabled = true;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTimeFlickering)
        {  
            nextTimeFlickering = Time.time + flickeringWait;
            light.enabled = !light.enabled;
        }
    }
}
