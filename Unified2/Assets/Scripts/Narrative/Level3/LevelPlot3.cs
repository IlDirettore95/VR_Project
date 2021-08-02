using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlot3 : MonoBehaviour
{
    [SerializeField] private GameObject _enterDoor;
    private UnlockableDoorAnimation _enterDoorAnimation;

    // Start is called before the first frame update
    void Start()
    {
        _enterDoorAnimation = _enterDoor.GetComponentInChildren<UnlockableDoorAnimation>();
        _enterDoorAnimation.LockDoor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
