using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartOfLevel : MonoBehaviour
{
    private LevelSystem _levelSys;
    private GameObject _player;

    [SerializeField] private GameObject _spawnPoint;
    [SerializeField] private string _previousLevel;

    private void Start()
    {
        _levelSys = GameObject.FindObjectOfType<LevelSystem>();
        _player = GameObject.FindGameObjectWithTag("Player");

        if (_levelSys._currentLevel.ToString() != _previousLevel)
        {
            _player.transform.position = _spawnPoint.transform.position;
            _player.transform.rotation = _spawnPoint.transform.rotation;

            Initialize();

            StartCoroutine(EnableMovSys());

            //Vector3 pos = _spawnPoint.transform.position - _player.transform.position;

            //_player.transform.GetComponent<CharacterController>().Move(pos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _levelSys.NextLevel();

            Initialize();

            gameObject.SetActive(false);
        }
    }

    private void Initialize()
    {
        switch (_levelSys._currentLevel.ToString())
        {
            case "Level2":

                //gameObject.GetComponent<InitializeLevel2>().Initialize();

                break;
        }
    }

    // Wait 1 frame before enabling the movement system, in order to complete the player warp to the spawnpoint
    private IEnumerator EnableMovSys()
    {
        yield return null;

        if(_levelSys._currentLevel.ToString() != "Level1")
        {
            _player.transform.GetComponent<MovementSystem>().enabled = true;
        }

        gameObject.SetActive(false);
    }
}
