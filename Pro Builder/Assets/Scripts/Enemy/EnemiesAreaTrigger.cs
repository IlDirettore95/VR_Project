using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesAreaTrigger : MonoBehaviour
{
    private EnemiesManager enemyManager;
    public int areaID;

    // Start is called before the first frame update
    void Start()
    {
        enemyManager = GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            enemyManager.PopolateArea(areaID);
        }
    }
}
