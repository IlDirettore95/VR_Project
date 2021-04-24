using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, ReactiveObject
{
    /*
     * This class handle Player status and statistics like
     * health, stamina and jetpack's fuel, provides also aux methods for editing, monitoring and questioning
     * about player's status;
     * every time the player get hurt the Health regeneration Componenti is disabled and renabled reinitializing his function
    */

    //Player's stats
    [SerializeField] private float MaxHealth;
    [SerializeField] private float MaxStamina;
    [SerializeField] private float MaxFuel;
    [SerializeField] private float MaxEnergy;

    private float _health;
    private float _stamina;
    private float _fuel;
    private float _energy;

    private HealthRegeneration _healRegeneration;

    //player status
    private bool isAlive;

    // Start is called before the first frame update
    void Start()
    {
    
        _health = MaxHealth;
        _stamina = MaxStamina;
        _fuel = MaxFuel;
        _energy = MaxEnergy;

        isAlive = true;

        _healRegeneration = GetComponent<HealthRegeneration>();
    }

    public float GetMaxHealth() => MaxHealth;

    public float GetMaxStamina() => MaxStamina;

    public float GetMaxFuel() => MaxFuel;

    public float GetMaxEnergy() => MaxEnergy;

    public float GetHealth() => _health;

    public float GetStamina() => _stamina;

    public float GetFuel() => _fuel;

    public float GetEnergy() => _energy;

    public void SetMaxHealth(float mh) => MaxHealth = mh;

    public void SetMaxStamina(float ms) => MaxStamina = ms;

    public void SetMaxFuel(float mf) => MaxFuel = mf;

    public void SetMaxEnergy(float me) => MaxEnergy = me;

    public bool IsAlive() => isAlive;

    public bool HasEnoughStamina() => _stamina > 0;

    public bool HasEnoughFuel() => _fuel > 0;

    public bool HasEnoughEnergy() => _energy > 0;

    public bool IsFullHealth() => _health == MaxHealth;

    public bool IsFullStamina() => _stamina == MaxStamina;

    public bool IsFullFuel() => _fuel == MaxFuel;

    public bool IsFullEnergy() => _energy == MaxEnergy;

    //If a player's health drops below 0 the player should be considered alive
    public void Hurt(float damage)
    {
        if (isAlive)
        {
            _health -= damage;
            if (_health < 0) _health = 0;
            if (_health == 0) isAlive = false;
       
            _healRegeneration.enabled = false;
            _healRegeneration.enabled = true;
        }
    }

    public void Heal(float cure)
    {

        _health += cure;
        if (_health > MaxHealth) _health = MaxHealth;   
    }

    public void ConsumeStamina(float consumed)
    {
        _stamina -= consumed;
        if (_stamina < 0) _stamina = 0;
    }

    public void RecoverStamina(float recovery)
    {
        _stamina += recovery;
        if (_stamina > MaxStamina) _stamina = MaxStamina;  
    }

    public void ConsumeFuel(float discharge)
    {
        _fuel -= discharge;
        if (_fuel < 0) _fuel = 0;
    }

    public void RecoverFuel(float charge)
    {
        _fuel += charge;
        if (_fuel > MaxFuel) _fuel = MaxFuel;
    }

    public void ConsumeEnergy(float discharge)
    {
        _energy -= discharge;
        if (_energy < 0) _energy = 0;
    }

    public void RecoverEnergy(float charge)
    {
        _energy += charge;
        if (_energy > MaxEnergy) _energy = MaxEnergy;
    }



    public void reset()
    {
        _health = MaxHealth;
        _stamina = MaxStamina;
        _fuel = MaxFuel;
        _energy = MaxEnergy;
        isAlive = true;
    }

    public void ReactToAttraction(float attractionSpeed) { }

    public void ReactToReleasing() { }

    public void ReactToLaunching(float launchingSpeed) { }

    public bool IsDestroyed() { return false; }

    public void ReactToExplosion(float damage)
    {
        Hurt(damage);
    }
}
