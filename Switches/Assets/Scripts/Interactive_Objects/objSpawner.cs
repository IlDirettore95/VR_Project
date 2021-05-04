using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objSpawner : MonoBehaviour, InteractableObject
{
    public GameObject objToSpawn;
    private bool isEnabled = false;
    public Transform positionSpawn;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {

            GameObject duplicate = Instantiate(objToSpawn, positionSpawn.position, positionSpawn.rotation);
            duplicate.GetComponent<Renderer>().enabled = true;
            duplicate.GetComponent<Rigidbody>().useGravity = true;

            isEnabled = false;
        }
    }

    public void setFalse()
    {
        isEnabled = false;
    }

    public void setTrue()
    {
        isEnabled = true;
    }
}
