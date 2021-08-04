using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
  
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
        GameObject go = other.gameObject;
        if (go != null && (go.tag.Equals("Player") || go.GetComponentInParent<ReactiveBox>() || go.GetComponentInParent<ReactiveFan>() || go.GetComponentInParent<ReactiveGrid>()) )
        {
            go.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject go = other.gameObject;
        if (go != null && (go.tag.Equals("Player") || go.GetComponentInParent<ReactiveBox>() || go.GetComponentInParent<ReactiveFan>() || go.GetComponentInParent<ReactiveGrid>()) )
        {
            go.transform.SetParent(null);
        }
    }
}
