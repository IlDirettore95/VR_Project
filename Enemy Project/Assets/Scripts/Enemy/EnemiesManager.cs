using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    private int maxDronePrefabs;
    public GameObject[] enemiesPrefabs;  //Enemies prefabs. Each element contain a different prefabs.
    private Dictionary<int, List<GameObject>> enemiesPool;  //Pool of all the enemies available. Each element contains an array of enemies of the same type.
    private Dictionary<int, List<GameObject>> enemiesArea;  //Each element of this table cointains the enemies present in an area.
    private Dictionary<int, List<SpawnPoint>> spawnPoints;  //Each element of this table cointains the list of spawnPoints related to the same area

    // Start is called before the first frame update
    void Start()
    {
        enemiesPool = new Dictionary<int, List<GameObject>>();
        spawnPoints = new Dictionary<int, List<SpawnPoint>>();
        enemiesArea = new Dictionary<int, List<GameObject>>();

        GameObject spawnpoints = GameObject.FindGameObjectWithTag("SpawnPoints");
        SpawnPoint[] spawnpoint = spawnpoints.GetComponentsInChildren<SpawnPoint>();
        foreach(SpawnPoint sp in spawnpoint)
        {
            spawnPoints[sp.GetAreaID()].Add(sp);
        }
        
        for(int i = 0; i < enemiesPrefabs.Length; i++)
        {
            for(int j = 0; j < maxDronePrefabs; j++)
            {
                GameObject enemy = Instantiate(enemiesPrefabs[i], transform.position, transform.rotation);
                enemy.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InstantiatePrefabs()
    {
        //Instantiate the maximum enemy prefabs available for the scene


    }

    public void TriggerArea(int areaID)
    {
        //Trigger all the enemies in the area identified by areaID.

        /*
        EnemiesArea ea = enemiesArea[areaID];
        List<GameObject> enemies = ea.GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<IEnemy>().BeTriggered();
        }
        */
    }

    public void PopolateArea(int areaID)
    {
        //Instantiate the enemies assigned in the area identified by areaID.
        //Reactivate the behavior of previously killed enemies.

        foreach(SpawnPoint sp in spawnPoints[areaID])
        {
            GameObject enemy = enemiesPool[sp.GetEnemyType()][0];
            enemy.transform.position = sp.transform.position;
            enemy.SetActive(true);
            enemy.GetComponent<IEnemy>().Revive();
        }


        /*
        EnemiesArea ea = enemiesArea[areaID];
        List<GameObject> sp = enemiesArea[areaID].GetSpawnPoints();
        List<GameObject> enemies = ea.GetEnemies();
        for(int i = 0; i < enemies.Capacity; i++)
        {
            enemies[i].transform.position = sp[i].transform.position;
            enemies[i].GetComponent<DroneV3>().Revive();
            enemies[i].SetActive(true);
            
        }
        */
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
