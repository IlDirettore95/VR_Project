using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableDoorAnimation : MonoBehaviour
{
    private Animator _animator;

    private enum Type
    {
        OneSide,
        DoubleSide
    }

    [SerializeField] private Type _type;

    [SerializeField] private Material _enabledMat;
    [SerializeField] private Material _disabledMat;

    //Light1 is the default one side light
    [SerializeField] private Light _enabledLight1;
    [SerializeField] private Light _disabledLight1;

    [SerializeField] private Light _enabledLight2;
    [SerializeField] private Light _disabledLight2;

    [SerializeField] private ReflectionProbe _refProb1;
    [SerializeField] private ReflectionProbe _refProb2;
    
    private AudioSource _audioSource;
    [SerializeField] private AudioClip doorOpening;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        UnLockDoor();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl != null && pl.tag.Equals("Player"))
        {
            _animator.SetBool("isOpening", true);
            _audioSource.PlayOneShot(doorOpening);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject pl = other.gameObject;
        if (pl != null && pl.tag.Equals("Player"))
        {
            _animator.SetBool("isOpening", false);
            _audioSource.PlayOneShot(doorOpening);
        }
    }
    public void LockDoor()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;

        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _disabledMat;

        _enabledLight1.enabled = false;
        _disabledLight1.enabled = true;

        _refProb1.RenderProbe();

        if(_type == Type.DoubleSide)
        {
            _enabledLight2.enabled = false;
            _disabledLight2.enabled = true;

            _refProb2.RenderProbe();
        }
    }

    public void UnLockDoor()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;

        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _enabledMat;

        _enabledLight1.enabled = true;
        _disabledLight1.enabled = false;

        _refProb1.RenderProbe();

        if (_type == Type.DoubleSide)
        {
            _enabledLight2.enabled = true;
            _disabledLight2.enabled = false;

            _refProb2.RenderProbe();
        }
    }
}
