using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PerceptionIA))]
[RequireComponent(typeof(Rigidbody))]
public class ReactionIA : MonoBehaviour
{
    PerceptionIA _perception;
    Rigidbody _rb;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        _perception = GetComponent<PerceptionIA>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_perception.IsAllerted())
        {
            //Just stand
        }
        else
        {

            transform.LookAt(_perception.GetPlayerPosition());
            _rb.MovePosition(transform.forward*Time.deltaTime);
        }
    }
}
