using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A reactive object is an object which reacts to player's gravity powers
 */
public interface ReactiveObject : ReactiveEntity
{
    void ReactToAttraction(float attractionSpeed); //Gravity power

    void ReactToReleasing(); //Gravity power

    void ReactToLaunching(float launchingSpeed); //Gravity power

    bool IsDestroyed(); //The object may be destroyed

    bool IsAttracted();
}
