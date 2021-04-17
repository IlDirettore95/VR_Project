using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    
    int GetID();

    void SetID(int id);

    int GetAreaID();

    void SetAreaID(int id);

    void Initialize();

    void Hurt(float damage); //Take damage and decrease enemy healt

    void Revive();

    void BeTriggered(); //Transit to the hostile state and start to chase the player

    void MoveTo(Vector3 point); //Move the enemy from the current position to point position in input

    void Patrol(Vector3[] path); //Patrol the path described by the array of position in input
}
