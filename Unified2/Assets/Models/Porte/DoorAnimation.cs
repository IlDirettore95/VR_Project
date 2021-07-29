using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    private Animator _animator;

    [SerializeField] private Material _enabledMat;
    [SerializeField] private Material _disabledMat;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private void OnDisable()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;

        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _disabledMat;
    }

    private void OnEnable()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }
}
