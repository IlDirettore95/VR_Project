using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConrollerN : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject enemyPrefab;
    private GameObject[] _enemies;
    public int enemiesCount;
    void Start()
    {
        _enemies = new GameObject[enemiesCount];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _enemies.Length; i++)
        {
            if (_enemies[i] == null)
            {
                _enemies[i] = Instantiate(enemyPrefab) as GameObject;
                _enemies[i].transform.position = new Vector3(Random.Range(1f,5f), 1f, Random.Range(1f,5f));
                float angle = Random.Range(0, 360);
                _enemies[i].transform.Rotate(0, angle, 0);
            }
        }
    }
}
