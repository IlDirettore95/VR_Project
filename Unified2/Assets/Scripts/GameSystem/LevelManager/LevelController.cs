using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _checkPoint;

    // Start is called before the first frame update
    void Start()
    {
        GameObject _player = Instantiate(_playerPrefab, _checkPoint.transform.position, _checkPoint.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
