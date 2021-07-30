﻿using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class PlayFootstepsSound : MonoBehaviour
{
    
   

    [SerializeField] private AudioClip footStepSound;
    
    [SerializeField] private AudioClip jumpSound;

    [SerializeField] private AudioClip jetpackSound;

    [SerializeField] private AudioClip attractSound;

    [SerializeField] private AudioClip releaseSound;

    [SerializeField] private AudioClip throwingSound;

    [SerializeField] private AudioClip hurtSound;

    [SerializeField] private AudioClip deathSound;
    
    
    
    //we can have separate audio channels for playing, looping and stopping audio clips
    private AudioSource[] _audioSources;

    private MovementSystem movementSystem;

    private PlayerStatus _playerStatus;

    private GravityPower _gravityPower;

    private PlayerAnimationController _playerAnimationController;

    private Jetpack _jetpack;
    
    private float _footStepSoundLenght;

    private bool _step;

    private bool _jump;

    private bool wasOnJetpack;

    private bool wasAttracting;

    private bool wasReleasing;

    private bool wasThrowing;
    
    private CharacterController _charController;
    
    // Start is called before the first frame update
    void Start()
    {
        movementSystem = GetComponent<MovementSystem>();
        _jetpack = GetComponent<Jetpack>();
        _charController = GetComponent<CharacterController>();
        _gravityPower = GetComponentInChildren<GravityPower>();
        _playerAnimationController = GetComponentInChildren<PlayerAnimationController>();
        _audioSources = GetComponents<AudioSource>();
        
        //primo gestisce camminata e salto, secondo il jetpack
        _step = true;
        _jump = true;
        //jetpack
        wasOnJetpack = false;
        //gravity
        wasAttracting = false;
        wasReleasing = false;
        wasThrowing = false;
            _footStepSoundLenght = 1.8f;
        
    }

    // Update is called once per frame
    void Update()
    {
        //sounds from movement system eg. walking, crouching, running & jumping
        if (movementSystem.enabled)
        {

            if ((movementSystem.walking||movementSystem.running) && _step)
            {
                _audioSources[0].PlayOneShot(footStepSound);
                StartCoroutine(WaitForFootSteps(_footStepSoundLenght));
            }
            if(movementSystem.wasGrounded && movementSystem.hasJumped && !movementSystem.isGrounded)
            {
                _audioSources[0].PlayOneShot(jumpSound);
                //_jump = false;
                //StartCoroutine(WaitForJump());
                //Debug.Log("ho saltato");
               // Debug.Log("was grounded: "+movementSystem.wasGrounded);
               // Debug.Log("has jumped: "+movementSystem.hasJumped);
            }
            
            
            //sound of falling
            if (!movementSystem.wasGrounded && movementSystem.isGrounded)
            {
                _audioSources[0].PlayOneShot(footStepSound);
            }
            
        }

        //sounds from jetpack
            if (movementSystem.jetpack && !wasOnJetpack)
            {
                wasOnJetpack = true;
                _audioSources[1].PlayOneShot(jetpackSound);
                _audioSources[1].loop = true;


            }

            else if(movementSystem.falling && wasOnJetpack)
            {
                Debug.Log("stop the jetpack");
                _audioSources[1].Stop();
                _audioSources[1].loop = false;
                wasOnJetpack = false;
            }
            
          //sounds from gravity powers eg attracting, releasing & throwing

          if (_playerAnimationController.GetIsGrabbing() && !wasAttracting)
          {
              Debug.Log("attract");
              wasAttracting = true;
              wasReleasing = false;
              wasThrowing = false;
              _audioSources[2].loop = true;
              _audioSources[2].PlayOneShot(attractSound);
              
          }
          else if (_playerAnimationController.GetIsReleasing() && !wasReleasing)
          {
              Debug.Log("releasing");
              wasReleasing = true;
              wasAttracting = false;
              wasThrowing = false;
              _audioSources[2].Stop();
              _audioSources[2].loop = false;
              _audioSources[2].PlayOneShot(releaseSound);
              
          }
          
          else if (_playerAnimationController.GetIsThrowing() && !wasThrowing)
          {
              Debug.Log("throwing");
              wasReleasing = false;
              wasAttracting = false;
              wasThrowing = true;
              _audioSources[2].Stop();
              _audioSources[2].loop = false;
              StartCoroutine(WaitForThrowingAnimation());
              
          }
           
          
          //sounds from hurting and death

          
            
        
        
    }
    
    public void Hurt()
    {
        _audioSources[3].PlayOneShot(hurtSound);         
    }

    public void Death()
    {
        _audioSources[3].PlayOneShot(deathSound);
    }

    IEnumerator WaitForFootSteps(float stepLenght)
    {
        _step = false;
        
        yield return new WaitForSeconds(Mathf.Clamp(stepLenght/movementSystem.speed,0.2f,0.4f));
        _step = true;
    }

    IEnumerator WaitForThrowingAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        _audioSources[1].PlayOneShot(throwingSound);
    }

    IEnumerator WaitForJump()
    {
        _jump = false;
        yield return new WaitForSeconds(0.3f);
        _jump = true;
    }
}
