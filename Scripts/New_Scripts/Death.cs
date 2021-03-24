using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    private Vector3 target;
    private Quaternion rotation;
    public float deathHeight;
    //This scrièt will teleport the player to the starting point once he reached the deathHeight

    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (transform.position.y <= deathHeight)
        {
            transform.position = target;
            transform.rotation = rotation;
            GetComponent<FPSInput_Jump_Jetpack>().resetY();
        }
    }
}
