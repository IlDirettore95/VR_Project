using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeLevel2 : MonoBehaviour
{
    [SerializeField] private GameObject _enterDoor;

    public void Initialize()
    {
        _enterDoor.GetComponentInChildren<DoorAnimation>().enabled = false;
    }
}
