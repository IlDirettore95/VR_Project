using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Slider energyBar;
    [SerializeField] private Slider fuelBar;


    private PlayerStatus _playerStatus;

    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if(healthBar.enabled) healthBar.value = _playerStatus.GetHealth() * 100 / _playerStatus.GetMaxHealth();
        if(staminaBar.enabled) staminaBar.value = _playerStatus.GetStamina() * 100 / _playerStatus.GetMaxStamina();
        if(fuelBar.enabled) fuelBar.value = _playerStatus.GetFuel() * 100 / _playerStatus.GetMaxFuel();
        if(energyBar.enabled) energyBar.value = _playerStatus.GetEnergy() * 100 / _playerStatus.GetMaxEnergy();
    }

    public void ActiveBar(int index, bool active)
    {
        if(index >= 0 && index <= 3)
        {
            switch (index)
            {
                case 0:
                    healthBar.gameObject.SetActive(active);
                    break;
                case 1:
                    staminaBar.gameObject.SetActive(active);
                    break;
                case 2:
                    energyBar.gameObject.SetActive(active);
                    break;
                case 3:
                    fuelBar.gameObject.SetActive(active);
                    break;
            }
        }
    }
}
