using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{
    [SerializeField] string _nextLevel;
    private bool _isLoaded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isLoaded)
        {
            _isLoaded = true;
            SceneManager.LoadSceneAsync(_nextLevel, LoadSceneMode.Additive);
        }
    }
}
