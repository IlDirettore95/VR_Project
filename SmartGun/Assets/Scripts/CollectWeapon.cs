using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectWeapon : MonoBehaviour
{
    public int weaponID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponManager wm = other.gameObject.GetComponentInChildren<WeaponManager>();
        if (wm != null)
        {
            wm.discoverWeapon(weaponID);
            Destroy(gameObject);
        }
        
        
    }
}
