using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    private Dictionary<int, List<GameObject>> enemiesPool;  //Pool of all the enemies available. Each element contains an array of enemies of the same type.
    private Dictionary<int, EnemiesArea> enemiesArea;  //Each element of this table cointains an EnemiesArea object.

    // Start is called before the first frame update
    void Start()
    {
        //enemiesPool.Add(0, new List<GameObject>());
        enemiesArea = new Dictionary<int, EnemiesArea>();

        GameObject area = GameObject.Find("Building");
        EnemiesArea ea = area.GetComponent<EnemiesArea>();
        
        /*GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject go in enemies)
        {
            ea.AddEnemy(go);
        }*/
        enemiesArea.Add(ea.GetID(), ea);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerArea(int areaID)
    {
        //Trigger all the enemies in the area identified by areaID.
        EnemiesArea ea = enemiesArea[areaID];
        List<GameObject> enemies = ea.GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<IEnemy>().BeTriggered();
        }
    }

    public void PopolateArea(int areaID)
    {
        //Instantiate the enemies assigned in the area identified by areaID.
        //Reactivate the behavior of previously killed enemies.
        EnemiesArea ea = enemiesArea[areaID];
        List<GameObject> sp = enemiesArea[areaID].GetSpawnPoints();
        List<GameObject> enemies = ea.GetEnemies();
        for(int i = 0; i < enemies.Capacity; i++)
        {
            enemies[i].transform.position = sp[i].transform.position;
            enemies[i].GetComponent<DroneV3>().Revive();
            enemies[i].SetActive(true);
            
        }
    }

    public void CollectEnemy(GameObject enemy)
    {
        //When an enemy dies, this method move the enemy game object to an hidden space point in the scene, in order to be reallocated in the future.
        //At the same time the enemy behaviour is disactivated.
        //enemy.GetComponent<Renderer>().enabled = false;
        enemy.SetActive(false);
        //enemy.transform.position = transform.position;
        //enemiesPool[0].Add(enemy);
    }
}
