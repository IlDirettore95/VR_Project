using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private Animator _animator;
    private AudioSource _audioSource;
    private bool _isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl != null)
        {
            if (!_isOpen)
            {
                _animator.SetBool("isOpening", true);
                _audioSource.PlayDelayed(0.6f);
                _isOpen = true;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl != null)
        {
            _animator.SetBool("isOpening", false);
            _audioSource.PlayDelayed(0.35f);
            _isOpen = false;
        }
    }
}
