using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ReactiveEnemy : ReactiveObject
{
    void ReactToExplosion(float damage);
}
