using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public int[] maxEnemiesPrefabs;

    public GameObject[] enemiesPrefabs;  //Enemies prefabs. Each element contain a different prefabs.
    private Dictionary<int, List<GameObject>> enemiesPool;  //Pool of all the enemies available. Each element contains an array of enemies of the same type.
    private Dictionary<int, List<GameObject>> enemiesArea;  //Each element of this table cointains the enemies present in an area.
    private Dictionary<int, List<SpawnPoint>> spawnPoints;  //Each element of this table cointains the list of spawnPoints related to the same area

    // Start is called before the first frame update
    void Start()
    {
        InstantiatePrefabs();

        InizializeSpawnpoint();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InstantiatePrefabs()
    {
        //Instantiate the maximum enemy prefabs available for the scene

        enemiesPool = new Dictionary<int, List<GameObject>>();

        for(int i = 0; i < maxEnemiesPrefabs.Length; i++)
        {
            enemiesPool[i] = new List<GameObject>();
            for(int j = 0; j < maxEnemiesPrefabs[i]; j++)
            {
                GameObject enemy = Instantiate(enemiesPrefabs[i], transform.position, transform.rotation);
                IEnemy e = enemy.GetComponent<IEnemy>();
                e.SetID(i);
                e.Initialize();
                enemy.SetActive(false);
                enemiesPool[i].Add(enemy);
            }
        }
    }

    private void InizializeSpawnpoint()
    {
        //Find all the spawnpoint present in the scene and collect them in a dictonary according to their areaID.

        spawnPoints = new Dictionary<int, List<SpawnPoint>>();
        enemiesArea = new Dictionary<int, List<GameObject>>();

        GameObject spawnpoints = GameObject.FindGameObjectWithTag("SpawnPoints");
        SpawnPoint[] spawnpoint = spawnpoints.GetComponentsInChildren<SpawnPoint>();
        foreach (SpawnPoint sp in spawnpoint)
        {
            int areaID = sp.GetAreaID();

            if (!spawnPoints.ContainsKey(areaID))
            {
                spawnPoints.Add(areaID, new List<SpawnPoint>());
            }

            spawnPoints[areaID].Add(sp);

            if(!enemiesArea.ContainsKey(areaID))
            {
                enemiesArea.Add(areaID, new List<GameObject>());
            }
        }
    }

    public void PopolateArea(int areaID)
    {
        //Instantiate the enemies assigned in the area identified by areaID.
        //Reactivate previously killed enemies.

        if (enemiesArea[areaID].Count != 0) return;  //If the player pass multiple time through the EnemiesAreaTrigger, this prevent the spawning of more enemies than expected

        foreach (SpawnPoint sp in spawnPoints[areaID])
        {
            int enemyType = sp.GetEnemyType();
            GameObject enemy = enemiesPool[enemyType][0];
            if(enemy != null)  //enemy is null if no more enemies of this type is currently available
            {
                enemiesArea[areaID].Add(enemy);
                enemiesPool[enemyType].RemoveAt(0);

                enemy.transform.position = sp.transform.position;
                enemy.GetComponent<IEnemy>().SetAreaID(areaID);
                enemy.SetActive(true);
                enemy.GetComponent<IEnemy>().Revive();
            }
        }
    }

    public void TriggerArea(int areaID)
    {
        //Trigger all the enemies in the area identified by areaID.

        
    }

    public void CollectEnemy(GameObject enemy)
    {
        //When an enemy dies, this method disables it and make it available in the pool.

        enemy.SetActive(false);

        IEnemy e = enemy.GetComponent<IEnemy>();
        int enemyID = e.GetID();
        int areaID = e.GetAreaID();
        enemiesPool[enemyID].Add(enemy);
        enemiesArea[areaID].Remove(enemy);
    }
}
