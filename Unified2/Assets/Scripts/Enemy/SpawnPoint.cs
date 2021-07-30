using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public int areaID;

    public int enemyType;

    public int GetAreaID()
    {
        return areaID;
    }

    public int GetEnemyType()
    {
        return enemyType;
    }
}
