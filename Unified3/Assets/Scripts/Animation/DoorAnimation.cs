using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl != null && pl.tag.Equals("Player"))
        {
            _animator.SetBool("isOpening", true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl != null && pl.tag.Equals("Player"))
        {
            _animator.SetBool("isOpening", false);
        }
    }
}
