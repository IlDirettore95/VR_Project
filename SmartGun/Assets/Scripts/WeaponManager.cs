using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private int numberOfweapons = 2;
    private PlayerWeapon[] weapons;
    private int currentWeapon;
    private const int gravityGunID = 0;
    private const int smartGunID = 1;
    

    // Start is called before the first frame update
    void Start()
    {
        weapons = new PlayerWeapon [numberOfweapons];
        weapons[0] = GetComponent<Shooter>();
       weapons[1] = GetComponent<SmartShooter>();

        setCurrentWeapon(0);

    }

    public void setCurrentWeapon(int indexOfWeapon)
    {
        
        //design by contract: una sola arma per volta 
        weapons[currentWeapon].enabled = false;//disabilito arma corrente prima di sostituirla
        
        currentWeapon = indexOfWeapon;
        
        weapons[currentWeapon].enabled = true;// abilito arma nuova
    }
    
    void Update()
    {
        //carico 
        if (currentWeapon!= gravityGunID && Input.GetKeyDown(KeyCode.Alpha1) )
        {
            setCurrentWeapon(gravityGunID);
        }
        
        else if (currentWeapon != smartGunID && Input.GetKeyDown(KeyCode.Alpha2))
        {
            setCurrentWeapon(smartGunID);
        }
    }
}
