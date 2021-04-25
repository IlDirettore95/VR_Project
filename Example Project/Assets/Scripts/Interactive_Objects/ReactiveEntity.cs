using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Is an entity which reacts to world's event 
public interface ReactiveEntity
{
    void ReactToExplosion(float damage, float power, Vector3 center, float radius); //Spider
}
