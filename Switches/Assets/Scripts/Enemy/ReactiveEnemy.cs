using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ReactiveEnemy 
{
    
    void reactToExplosion(float damage);

    void reactToFire(float damage);

    void reactToFan(float damage);
}
