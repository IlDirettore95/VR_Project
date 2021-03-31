using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    public GameObject leftDoor;

    public GameObject rightDoor;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.GetComponent<GameObject>();
        if (obj != null)
            openDoor();
        
        
    }

    private void openDoor()
    {
        leftDoor.transform.Translate(-1.25F,0,0);
        rightDoor.transform.Translate(1.25F,0,0);
    }
}
