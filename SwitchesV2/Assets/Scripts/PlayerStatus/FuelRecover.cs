using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class handles fuel recovery.
 * Once all fuel is recovered this script disables itself
 */
public class FuelRecover : MonoBehaviour
{
    public float recoverRate;

    private PlayerStatus _playerStatus;


    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        _playerStatus.RecoverFuel(recoverRate * Time.deltaTime);
        if (_playerStatus.IsFullFuel()) enabled = false;
    }
}
