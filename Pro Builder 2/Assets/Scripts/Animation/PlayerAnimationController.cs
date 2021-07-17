using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;
    GravityPower powers;
    MovementSystem moveSys;

    public bool GetIsGrabbing() => animator.GetBool("IsGrabbing");
    public bool GetIsThrowing() => animator.GetBool("IsThrowing");
    public bool GetIsReleasing() => animator.GetBool("IsReleasing");

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

    public void Attract()
    {
        animator.SetBool("IsGrabbing", true);
    }

    public void StopAttracting()
    {
        animator.SetBool("IsGrabbing", false);
    }

    public void Throw()
    {
        animator.SetBool("IsThrowing", true);
        animator.SetBool("IsGrabbing", false);
    }

    public void StopThrowing()
    {
        animator.SetBool("IsThrowing", false);
    }

    public void Release()
    {
        animator.SetBool("IsReleasing", true);
        animator.SetBool("IsGrabbing", false);
    }

    public void StopReleasing()
    {
        animator.SetBool("IsReleasing", false);
    }

    public void NextState()
    {
        if (moveSys.running && !powers.IsAttracting())
        {
            animator.SetBool("IsRunning", true);
        }
        else animator.SetBool("IsRunning", false);

        if (moveSys.falling)
        {
            animator.SetBool("IsRunning", false);

            animator.SetBool("IsJumping", true);
        }
        else animator.SetBool("IsJumping", false);
    }  
}
