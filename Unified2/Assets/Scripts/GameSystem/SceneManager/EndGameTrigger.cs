using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    public LevelSystem levelSys;

    private void OnTriggerEnter(Collider other)
    {
        levelSys.EndGame();
    }
}
