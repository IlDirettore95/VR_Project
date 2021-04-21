using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A reactive object is an object which reacts to player's gravity powers
 */
public interface ReactiveObject 
{
    void ReactToAttraction(float attractionSpeed);

    void ReactToRepulsing();

    void ReactToReleasing();

    void ReactToLaunching(float launchingSpeed);

    void ReactToIncreasing();

    void ReactToDecreasing();
    bool IsDestroyed();
}
