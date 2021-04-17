using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class handles stamina recovery.
 * When this class is enabled it means that stamina can be recovered after the cooldown.
 * Once all the stamina is recovered this script disables itself
 */
public class StaminaRecover : MonoBehaviour
{
    public float recoverCooldown;
    public float recoverRate;
    private float nextTimeRecover;

    private PlayerStatus _playerStatus;

    //This script must be enabled when conditions for stamina's recovery are in place.
    private void OnEnable()
    {
        nextTimeRecover = Time.time + recoverCooldown;
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = GetComponent<PlayerStatus>();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextTimeRecover && _playerStatus.IsAlive()) _playerStatus.RecoverStamina(recoverRate * Time.deltaTime);
        if (_playerStatus.IsFullStamina()) enabled = false;
    }
}
