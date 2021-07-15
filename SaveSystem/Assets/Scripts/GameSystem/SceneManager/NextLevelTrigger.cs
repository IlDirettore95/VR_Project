using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public GameSystem gameSys;
    public string nextLevel;

    private void OnTriggerEnter(Collider other)
    {
        gameSys.NextLevel();
    }
}
