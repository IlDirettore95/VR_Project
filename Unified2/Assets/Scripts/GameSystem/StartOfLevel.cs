using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartOfLevel : MonoBehaviour
{
    private LevelSystem _levelSys;
    private GameObject _player;
    private GameObject _spawnPoint;
    
    [SerializeField] private GameObject _enterDoor;
    [SerializeField] private string _previousLevel;

    private bool _isUnLoaded = false;

    private void Start()
    {
        _levelSys = GameObject.FindObjectOfType<LevelSystem>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _spawnPoint = GameObject.FindGameObjectWithTag("CheckPoint");

        Debug.Log(_levelSys._currentLevel.ToString());

        if (_levelSys._currentLevel.ToString() != _previousLevel)
        {
            //_player.transform.position = _checkPoint.transform.position;
            _player.transform.rotation = _spawnPoint.transform.rotation;

            //_player.transform.GetComponent<MovementSystem>().enabled = true;

            Vector3 pos = _spawnPoint.transform.position - _player.transform.position;

            Debug.Log(pos);
            Debug.Log(_player.transform.position);

            _player.transform.GetComponent<CharacterController>().Move(pos);

            _enterDoor.GetComponentInChildren<DoorAnimation>().enabled = false;

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string _currentLevel = _levelSys._currentLevel.ToString();

        if (other.CompareTag("Player"))
        {
            _enterDoor.GetComponentInChildren<DoorAnimation>().enabled = false;

            _levelSys.NextLevel();

            gameObject.SetActive(false);
        }
    }
}
