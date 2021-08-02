using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{
    private LevelSystem _levelSys;
    private bool _isLoaded = false;

    private void Start()
    {
        _levelSys = GameObject.FindObjectOfType<LevelSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isLoaded)
        {
            _levelSys.EndLevel();

            _isLoaded = true;

            gameObject.SetActive(false);
        }
    }
}
