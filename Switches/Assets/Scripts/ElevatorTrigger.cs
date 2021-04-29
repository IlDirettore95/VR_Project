using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("collision");
        GameObject obj = other.gameObject;
        if (obj != null && obj.tag.Equals("Player"))
        {
            Debug.Log("ciao");
            obj.transform.SetParent(transform);
            
        }

        if (obj.GetComponent<ReactiveBox>())
        {
            obj.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject player = other.gameObject;
        if (player != null && player.tag.Equals("Player"))
        {
            player.transform.SetParent(null);
            
        }
    }
}
