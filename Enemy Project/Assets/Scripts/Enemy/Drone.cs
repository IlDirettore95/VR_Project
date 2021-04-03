using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drone : MonoBehaviour
{
    public float movementSpeed = 4f;

    public Transform playerTransform;
    private GameObject player;
    Rigidbody r;

    private bool isStable;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.useGravity = false;
        player = GameObject.Find("Player");
        playerTransform = player.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject target = null;
        Collider[] col = Physics.OverlapSphere(transform.position, 5.0f);
        if (col.Length != 0)
        {
            //Trova la collisione più vicina e quella più lontana
            float max = (col[0].ClosestPoint(transform.position) - transform.position).magnitude;
            int maxIndex = 0;
            float min = (col[0].ClosestPoint(transform.position) - transform.position).magnitude;
            int minIndex = 0;

            for (int i = 0; i < col.Length; i++)
            {
                if(col[i].transform.gameObject.GetComponent<PlayerStatus>() != null)
                {
                    target = col[i].transform.gameObject;
                }
                if (!col[i].name.Equals("Drone"))
                {
                    float dist = (col[i].ClosestPoint(transform.position) - transform.position).magnitude;
                    //Debug.Log("Hit oggetto " + col[i].name + " distante " + dist);
                    if (max < dist)
                    {
                        max = dist;
                        maxIndex = i;
                    }
                    if (min > dist)
                    {
                        min = dist;
                        minIndex = i;
                    }
                }
            }

            /*
            //Calcola la media tra il punto più distante e il più vicino
            float media = (max + min)/2;
            //Tenta di spostarsi verso il punto medio della distanza tra l'oggetto più distante e il più vicino

            //Debug.Log("Max: " + max + ", Min: " + min + ", Media: " + media);

            if ((col[maxIndex].ClosestPoint(transform.position) - transform.position).magnitude > media)
            {
                Debug.Log("Dist: " + (col[maxIndex].ClosestPoint(transform.position) - transform.position).magnitude + ",Media: " + media);
                transform.LookAt(col[maxIndex].transform);
                float forzaFrenante = r.velocity.magnitude;
                //Debug.Log("Forza: " + forzaFrenante);
                r.AddRelativeForce(0, 0, 1);
                r.velocity.Normalize();
                //r.AddRelativeForce(0, 0, -forzaFrenante);
            }
            else
            {
                Debug.Log("FREENA!");
                r.velocity = new Vector3(0,0,0);
                r.inertiaTensor = new Vector3(0, 0, 0);
                
            }
            */
            
            r.AddForce((col[maxIndex].transform.position - transform.position).normalized);
            if(target != null)
            {
                transform.LookAt(target.transform.position);
                r.AddRelativeForce(0, 0, 1);
            }
            else
            {
                transform.LookAt(col[maxIndex].transform.position);
            }
            //r.velocity.Normalize();
        }
    }
}
