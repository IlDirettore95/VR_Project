using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private int numberOfweapons = 2;
    private PlayerWeapon[] weapons;
    private bool[] isDiscovered;
    private GameObject[] models3d;
    private int currentWeapon;
    private const int gravityGunID = 0;
    private const int smartGunID = 1;

//modelli 3d
    public GameObject gravitygun;
    public GameObject smartgun;
    

    // Start is called before the first frame update
    void Start()
    {
        weapons = new PlayerWeapon [numberOfweapons];
        isDiscovered = new bool[numberOfweapons];
        models3d = new GameObject[numberOfweapons];
        
        weapons[0] = GetComponent<Shooter>();
       weapons[1] = GetComponent<SmartShooter>();

       isDiscovered[0] = false;
       isDiscovered[1] = false;

       models3d[0] = gravitygun;
       models3d[1] = smartgun;

       //setCurrentWeapon(0);

    }

    public void setCurrentWeapon(int indexOfWeapon)
    {
        
        //design by contract: una sola arma per volta 
        weapons[currentWeapon].enabled = false;//disabilito arma corrente prima di sostituirla
        models3d[currentWeapon].GetComponent<Renderer>().enabled = false;
        
        currentWeapon = indexOfWeapon;
        
        weapons[currentWeapon].enabled = true;// abilito arma nuova
        models3d[currentWeapon].GetComponent<Renderer>().enabled = true;

    }

    public void discoverWeapon(int weaponID)
    {
        isDiscovered[weaponID] = true;
        setCurrentWeapon(weaponID);
    }
    
    void Update()
    {
        //carico 
        if (currentWeapon!= gravityGunID && Input.GetKeyDown(KeyCode.Alpha1) )
        {
            bool discovered = isDiscovered[gravityGunID];
            if(discovered)
                setCurrentWeapon(gravityGunID);
        }
        
        else if (currentWeapon != smartGunID && Input.GetKeyDown(KeyCode.Alpha2))
        {
            bool discovered = isDiscovered[smartGunID];
            if(discovered)
                setCurrentWeapon(smartGunID);
        }
    }
}
