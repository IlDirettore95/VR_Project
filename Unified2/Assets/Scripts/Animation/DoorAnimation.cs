using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private Animator _animator;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl != null && pl.tag.Equals("Player"))
        {
            _animator.SetBool("isOpening", true);
            _audioSource.PlayDelayed(0.5f);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl != null && pl.tag.Equals("Player"))
        {
            _animator.SetBool("isOpening", false);
            _audioSource.PlayDelayed(0.1f);
        }
    }
}
