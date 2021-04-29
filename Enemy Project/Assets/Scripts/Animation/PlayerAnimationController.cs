using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;
    GravityPower powers;
    MovementSystem moveSys;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        powers = GetComponentInChildren<GravityPower>();
        moveSys = GetComponent<MovementSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextState()
    {
        if (powers.IsAttracting())
        {
            animator.SetBool("IsGrabbing", true);
        }
        else animator.SetBool("IsGrabbing", false);

        if (powers.IsLaunching())
        {
            animator.SetBool("IsThrowing", true);
        }
        else animator.SetBool("IsThrowing", false);

        if (powers.IsReleasing())
        {
            animator.SetBool("IsReleasing", true);
        }
        else animator.SetBool("IsReleasing", false);

        if (moveSys.running && !powers.IsAttracting())
        {
            animator.SetBool("IsRunning", true);
        }
        else animator.SetBool("IsRunning", false);

        if (moveSys.hasJumped)
        {
            animator.SetBool("IsRunning", false);
        }
    }
}
