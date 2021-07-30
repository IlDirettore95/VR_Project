using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public LevelSystem levelSys;
    public string nextLevel;

    private void OnTriggerEnter(Collider other)
    {
        levelSys.NextLevel();
    }
}
