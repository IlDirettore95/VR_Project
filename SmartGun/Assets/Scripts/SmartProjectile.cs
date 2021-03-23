using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartProjectile : MonoBehaviour
{
    public float speed;
    private bool hasTarget = false;
    GameObject enemyTarget;
    private float damage = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyController target = collision.gameObject.GetComponent<EnemyController>();
        if (target != null)
        {
            target.Hurt(damage);
            Debug.Log("Target hit!");
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (!hasTarget)
        {
            FindTarget();
        }
        else
        {
            if(enemyTarget != null)
                transform.LookAt(enemyTarget.transform);
        }

    }

    void FindTarget()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, 5.0f);

        if (col.Length != 0)
        {
            List<GameObject> enemies = new List<GameObject>();
            GameObject hitObject;
            for (int i = 0; i < col.Length; i++)
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
            foreach (GameObject e in enemies)
            {
                if (c == r)
                {
                    enemyTarget = e;
                    //Debug.Log("Target Aquired!");
                    hasTarget = true;
                    break;
                }
                else c++;
            }
        }
    }
}
