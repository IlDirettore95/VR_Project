using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void Hurt(float damage);

    void TriggerNearbyEnemies();
}
