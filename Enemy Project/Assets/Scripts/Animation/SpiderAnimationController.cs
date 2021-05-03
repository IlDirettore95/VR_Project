using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAnimationController : MonoBehaviour
{
    Animator _animator;
    MiningSpider _spider;

    /*
    public enum SpiderState
    {
        Patrolling,
        Chasing,
        Attracted,
        Throwed,
        Exploding
    }

    SpiderState _currentState;
    */
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _spider = GetComponent<MiningSpider>();

        //_currentState = SpiderState.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAnimation()
    {
        switch (_spider.GetState())
        {
            case MiningSpider.SpiderState.Guarding:
                {
                    _animator.SetBool("IsGuarding", true);

                    break;
                }
            case MiningSpider.SpiderState.Patrolling:
                {
                    _animator.SetBool("IsPatrolling", true);

                    _animator.SetBool("IsGuarding", false);

                    break;
                }
            case MiningSpider.SpiderState.Chasing:
                {
                    _animator.SetBool("IsChasing", true);

                    _animator.SetBool("IsPatrolling", false);
                    _animator.SetBool("IsGuarding", false);
                    _animator.SetBool("IsExploding", false);
                    _animator.SetBool("IsAttracted", false);
                    _animator.SetBool("IsThrowed", false);

                    break;
                }
            case MiningSpider.SpiderState.Exploding:
                {
                    _animator.SetBool("IsExploding", true);

                    _animator.SetBool("IsPatrolling", false);
                    _animator.SetBool("IsGuarding", false);
                    _animator.SetBool("IsChasing", false);
                    _animator.SetBool("IsAttracted", false);
                    _animator.SetBool("IsThrowed", false);

                    break;
                }
            case MiningSpider.SpiderState.Attracted:
                {
                    _animator.SetBool("IsAttracted", true);

                    _animator.SetBool("IsPatrolling", false);
                    _animator.SetBool("IsGuarding", false);
                    _animator.SetBool("IsChasing", false);
                    _animator.SetBool("IsExploding", false);
                    _animator.SetBool("IsThrowed", false);

                    break;
                }
            case MiningSpider.SpiderState.Throwed:
                {
                    _animator.SetBool("IsThrowed", true);
                    
                    _animator.SetBool("IsPatrolling", false);
                    _animator.SetBool("IsGuarding", false);
                    _animator.SetBool("IsChasing", false);
                    _animator.SetBool("IsExploding", false);
                    _animator.SetBool("IsAttracted", false);

                    break;
                }
        }
    }
    /*
    public void Patrol()
    {
        _currentState = SpiderState.Patrolling;
    }

    public void Chase()
    {
        _currentState = SpiderState.Chasing;
    }

    public void Explode()
    {
        _currentState = SpiderState.Exploding;
    }

    public void Throwed()
    {
        _currentState = SpiderState.Throwed;
    }

    public void Attracted()
    {
        _currentState = SpiderState.Attracted;
    }
    */
}
