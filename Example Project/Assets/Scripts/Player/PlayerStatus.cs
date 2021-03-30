using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private Vector3 target;
    private Quaternion rotation;
    
    public float deathHeight;

    /*
     * This class handle Player status and statistics like
     * health, stamina and jetpack's fuel, provides also aux methods for editing, monitoring and questioning
     * about player's status;
    */
    //Player's stats
    [SerializeField] private float MaxHealth;
    [SerializeField] private float MaxStamina;
    [SerializeField] private float MaxFuel;
    private float _health;
    private float _stamina;
    private float _fuel;

    /*Life regeneration handling
     * After a cooldown, if the player has not been hurt he will start to regenerate
     */
    private float nextTimeRegeneration;
    public float regenerationCooldown;
    public float regenerationRate;

    //player status
    private bool isAlive;


    //This script will teleport the player to the starting point once he reached the deathHeight

    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rotation = transform.rotation;

        _health = MaxHealth;
        _stamina = MaxStamina;
        _fuel = MaxFuel;

        isAlive = true;
    }
    // Update is called once per frame
    private void Update()
    {
        if (isAlive && Time.time > nextTimeRegeneration) Heal(regenerationRate * Time.deltaTime);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (transform.position.y <= deathHeight)
        {
            transform.position = target;
            transform.rotation = rotation;
            reset();
            GetComponent<FPSInput_Jump_Jetpack>().resetY();
            GetComponent<MovementSystem>().reset();
        }
    }

    public float GetMaxHealth()
    {
        return MaxHealth;
    }

    public float GetMaxStamina()
    {
        return MaxStamina;
    }

    public float GetMaxFuel()
    {
        return MaxFuel;
    }

    public float GetHealth()
    {
        return _health;
    }

    public float GetStamina()
    {
        return _stamina;
    }

    public float GetFuel()
    {
        return _fuel;
    }

    public void SetMaxHealth(float mh)
    {
        MaxHealth = mh;
    }

    public void SetMaxStamina(float ms)
    {
        MaxStamina = ms;
    }

    public void SetMaxFuel(float mf)
    {
        MaxFuel = mf;
    }

    public void SetHealth(float h)
    {
        _health = h;
    }

    public void SetStamina(float s)
    {
        _stamina = s;
    }

    public void SetFuel(float f)
    {
        _fuel = f;
    }

    //If a player's health drops below 0 the player should be considered alive
    public void Hurt(float damage)
    {
        _health -= damage;
        if (_health < 0) _health = 0;
        if (_health == 0) isAlive = false;

        //Setting regeneration Cooldown
        if(isAlive) nextTimeRegeneration = Time.time + regenerationCooldown;
    }

    public void Heal(float cure)
    {
        if(isAlive)
        {
            _health += cure;
            if (_health > MaxHealth) _health = MaxHealth;
        }
        
    }

    public void ConsumeStamina(float consumed)
    {
        _stamina -= consumed;
        if (_stamina < 0) _stamina = 0;
    }

    public void RecoverStamina(float recovery)
    {
        if(isAlive)
        {
            _stamina += recovery;
            if (_stamina > MaxStamina) _stamina = MaxStamina;
        }   
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

    public bool IsAlive()
    {
        return isAlive;
    }

    public bool HasEnoughEnergy()
    {
        return _stamina > 0;
    }

    public bool HasEnoughFuel()
    {
        return _fuel > 0;
    }

    public bool IsFullHealth()
    {
        return _health == MaxHealth;
    }

    public bool IsFullStamina()
    {
        return _stamina == MaxStamina;
    }

    public bool IsFullFuel()
    {
        return _fuel == MaxFuel;
    }

    public void reset()
    {
        _health = MaxHealth;
        _stamina = MaxStamina;
        _fuel = MaxFuel;
        isAlive = true;
    }
}
