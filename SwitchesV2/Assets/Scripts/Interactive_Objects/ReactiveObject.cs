using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A reactive object is an object which reacts to player's gravity powers
 */
public interface ReactiveObject 
{
    void ReactToAttraction(float attractionSpeed);

   

    void ReactToReleasing();

    void ReactToLaunching(float launchingSpeed);



    void reactToFire(float damage);
 
    bool IsDestroyed();

    bool IsAttracted();
    void reactToExplosion(float damage);

    void reactToFan(Vector3 direction, float angularVelocity, float damage, bool isInBox);
}
