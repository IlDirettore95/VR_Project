using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionIA : MonoBehaviour
{
    public Transform _player;
    public float distanceThreashold;
    private bool _isAllerted;
    // Start is called before the first frame update
    void Start()
    {
        _isAllerted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetPlayerDistance() > distanceThreashold)
        {
            _isAllerted = false;
        }
        else
        {
            _isAllerted = true;
        }
        if(!_isAllerted)
        {
            GetComponent<Renderer>().material.color = Color.gray;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public bool IsAllerted()
    {
        return _isAllerted;
    }

    public Vector3 GetPlayerPosition()
    {
        return _player.transform.position;
    }

    public float GetPlayerDistance()
    {
        return Vector3.Distance(_player.transform.position, transform.position);
    }
}
