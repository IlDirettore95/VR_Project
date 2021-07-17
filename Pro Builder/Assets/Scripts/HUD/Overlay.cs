using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Slider fuelBar;
    [SerializeField] private Slider energyBar;

    private PlayerStatus _playerStatus;

    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = _playerStatus.GetHealth() * 100 / _playerStatus.GetMaxHealth();
        staminaBar.value = _playerStatus.GetStamina() * 100 / _playerStatus.GetMaxStamina();
        fuelBar.value = _playerStatus.GetFuel() * 100 / _playerStatus.GetMaxFuel();
        energyBar.value = _playerStatus.GetEnergy() * 100 / _playerStatus.GetMaxEnergy();
    }
}
