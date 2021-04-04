using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmStateController : MonoBehaviour
{
    Animator animator;
    Shooter shooter;
    MovementSystem moveSys;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        shooter = GetComponentInParent<Shooter>();
        moveSys = GetComponentInParent<MovementSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isRunning = moveSys.IsRunning();
        bool isGrabbing = shooter.GetIsAttracting();

        if (isRunning && !isGrabbing)
        {
            animator.SetBool("IsRunning", true);
        }
        else animator.SetBool("IsRunning", false);

        if (Input.GetKey(KeyCode.Mouse2))
        {
            animator.SetBool("OpenShield", true);
        }
        else animator.SetBool("OpenShield", false);
    }
}
