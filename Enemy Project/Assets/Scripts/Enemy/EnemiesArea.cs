using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesArea : MonoBehaviour
{
    public int ID; //Identifier of this enemiesArea
    public List<GameObject> spawnPoint;  //Array of all the spownpoint present in this area
    public List<GameObject> enemies;  //List of all the enemies assigned to this area

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetID()
    {
        return ID;
    }

    public List<GameObject> GetSpawnPoints()
    {
        return spawnPoint;
    }

    public List<GameObject> GetEnemies()
    {
        return enemies;
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
}
