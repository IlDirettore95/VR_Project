using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmStateController : MonoBehaviour
{
    Animator animator;
    Shooter shooter;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        shooter = GetComponentInParent<Shooter>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrabbing = shooter.GetIsAttracting();
        bool isThrowing = shooter.GetIsLaunching();
        bool isReleasing = shooter.GetIsReleasing();
        bool isRepulsing = shooter.GetIsRepulsing();

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

        if (isRepulsing)
        {
            animator.SetBool("IsRepulsing", true);
        }
        else animator.SetBool("IsRepulsing", false);
    }
}
