using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    public static List<Attractor> attractors;
    public Rigidbody rb;

    void FixedUpdate ()
    {
        //Attractor[] attractors = FindObjectsOfType<Attractor>();
        foreach (Attractor attractor in attractors)
        {
            if(attractor != this) Attract(attractor);
        }
    }

    void OnEnable()
    {
        if(attractors == null) attractors = new List<Attractor>();
        attractors.Add(this);
    }

    void onDisable()
    {
        attractors.Remove(this);
    }
    
    void Attract (Attractor objToAttract)
    {
        
        Rigidbody rbToAttract = objToAttract.rb;
        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;
        float forceMagnitude = (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }
}
