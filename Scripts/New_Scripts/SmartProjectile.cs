﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartProjectile : MonoBehaviour
{
    public float speed;
    public GameObject rayPoint;
    private bool hasTarget = false;
    GameObject enemyTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (!hasTarget)
        {
            Collider[] col = Physics.OverlapSphere(rayPoint.transform.position, 5.0f);
            
            if (col.Length != 0) 
            {
                List<GameObject> enemies = new List<GameObject>();
                GameObject hitObject;
                for(int i = 0; i < col.Length; i++)
                {
                    hitObject = col[i].transform.gameObject;
                    EnemyController target = hitObject.GetComponent<EnemyController>();
                    if (target != null)
                    {
                        enemies.Add(hitObject);
                    }
                }
                int c = 0;
                int r = Random.Range(0, enemies.Capacity);
                foreach(GameObject e in enemies)
                {
                    if (c == r)
                    {
                        enemyTarget = e;
                        Debug.Log("Target Aquired!");
                        hasTarget = true;
                        break;
                    }
                    else c++;
                }
            }
        }
        else
        {
            transform.LookAt(enemyTarget.transform);
        }

    }
}