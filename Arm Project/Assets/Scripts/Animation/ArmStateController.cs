using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmStateController : MonoBehaviour
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
        bool isGrabbing = shooter.GetIsAttracting();
        bool isThrowing = shooter.GetIsLaunching();
        bool isReleasing = shooter.GetIsReleasing();
        bool isRepulsing = shooter.GetIsRepulsing();
        bool isRunning = moveSys.IsRunning();

        if (isGrabbing)
        {
            animator.SetBool("IsGrabbing", true);
        }
        else animator.SetBool("IsGrabbing", false);

        if (isThrowing)
        {
            animator.SetBool("IsThrowing", true);
        }
        else animator.SetBool("IsThrowing", false);

        if (isReleasing)
        {
            animator.SetBool("IsReleasing", true);
        }
        else animator.SetBool("IsReleasing", false);
        /*
        if (isRepulsing)
        {
            animator.SetBool("IsRepulsing", true);
        }
        else animator.SetBool("IsRepulsing", false);

        if (isRunning && !isGrabbing)
        {
            animator.SetBool("IsRunning", true);
        }
        else animator.SetBool("IsRunning", false);

        if (Input.GetKeyDown(KeyCode.P))
        {
            animator.SetBool("FuckYou", true);
        }
        else animator.SetBool("FuckYou", false);
        */
    }
}
