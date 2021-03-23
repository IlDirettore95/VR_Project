using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private PlayerWeapon primaryWeapon;
    private PlayerWeapon currentWeapon;

    [SerializeField]
    private Transform weaponHolder;

    // Start is called before the first frame update
    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
