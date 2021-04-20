using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegeneration : MonoBehaviour
{
    public float regenerationCooldown;
    public float regenerationRate;
    private float nextTimeRegeneration;

    private PlayerStatus _playerStatus;

    private void OnEnable()
    {
        nextTimeRegeneration = Time.time + regenerationCooldown;
    }
    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = GetComponent<PlayerStatus>();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextTimeRegeneration && _playerStatus.IsAlive()) _playerStatus.Heal(regenerationRate * Time.deltaTime);
        if (_playerStatus.IsFullHealth()) enabled = false;
    }
}
