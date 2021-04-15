using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* In order to activate or deactivate jetpack in certain moments of the game,
 * it was implemented in a separated script, how ever it uses and is based on Movement System script
 */
public class Jetpack : MonoBehaviour
{
    //jetpack speeds
    public float jetpackSpeed;
    public float jetpackYSpeed;
    public float jetpackBuildUp;

    //Fuel handling for jetpack
    public float fConsumigRate;
    public float fActivationCost;

    private MovementSystem _movementSystem;
    private PlayerStatus _playerStatus;
    private FuelRecover _fuelRecover;

    private void Start()
    {
        _movementSystem = GetComponent<MovementSystem>();
        _playerStatus = GetComponent<PlayerStatus>();
        _fuelRecover = GetComponent<FuelRecover>();
    }
    // Update is called once per frame
    void Update()
    {
        //Grounded
        if (_movementSystem.isGrounded)
        {
            if (!_movementSystem.wasGrounded && !_playerStatus.IsFullFuel()) _fuelRecover.enabled = true;
        }
        //Not Grounded
        else
        {
            if (_movementSystem.wasGrounded) _fuelRecover.enabled = false;

            if (Input.GetButtonDown("Jump") && _playerStatus.HasEnoughFuel())
            {
                //Jetpack Activation
                _movementSystem.jetpack = true;
                _movementSystem.falling = false;

                //Lerping from speed to falling speed
                _movementSystem.SetSpeed(jetpackSpeed, jetpackBuildUp);

                //Setting Y Speed
                _movementSystem.SetYSpeed(jetpackYSpeed);

                _playerStatus.ConsumeFuel(fActivationCost);

                if (!_playerStatus.HasEnoughFuel())
                {
                    _movementSystem.falling = true;
                    _movementSystem.jetpack = false;
                }
            }
            else if (Input.GetButton("Jump") && _movementSystem.jetpack && _playerStatus.HasEnoughFuel())
            {
                //Jetpack

                //Lerping from speed to falling speed
                _movementSystem.SetSpeed(jetpackSpeed, jetpackBuildUp);
                //Setting Y Speed
                _movementSystem.SetYSpeed(jetpackYSpeed);

                _playerStatus.ConsumeFuel(fConsumigRate * Time.deltaTime);
                if (!_playerStatus.HasEnoughFuel())
                {
                    _movementSystem.falling = true;
                    _movementSystem.jetpack = false;
                }
            }
            else if (Input.GetButtonUp("Jump"))
            {
                //Jetpack disactivation
                _movementSystem.falling = true;
                _movementSystem.jetpack = false;
                _movementSystem.startFallingY = transform.position.y;
            }
        }
    }
}
