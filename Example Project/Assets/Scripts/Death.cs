using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    private Vector3 target;
    private Quaternion rotation;
    private float health = 20.0f;
    public float deathHeight;
    //This script will teleport the player to the starting point once he reached the deathHeight

    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Debug.Log("Health= " + health);
        if (transform.position.y <= deathHeight)
        {
            Debug.Log("SEI MORTO!");
            transform.position = target;
            transform.rotation = rotation;
            GetComponent<FPSInput_Jump_Jetpack>().resetY();
        }
    }

    public void Hurt(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;
        if (health == 0) Debug.Log("SEI MORTO!");
    }
}
