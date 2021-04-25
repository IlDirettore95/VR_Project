using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ReactiveObject 
{
    void ReactToAttraction(Vector3 target, float attractionSpeed);

    void ReactToRepulsing(Vector3 direction, float repulsingSpeed);

    void ReactToReleasing();

    void ReactToLaunching(Vector3 direction, float launchingSpeed);
}
