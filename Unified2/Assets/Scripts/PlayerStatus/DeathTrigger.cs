using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerStatus _playerStatus = other.GetComponent<PlayerStatus>();
        if(_playerStatus != null)
        {
            _playerStatus.Hurt(_playerStatus.GetMaxHealth());
        }
    }
}