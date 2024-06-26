﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This script handles dialogues which want to be displayed using the dialogue system 
 */
public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private DialogueTrigger _currentTrigger;

    [SerializeField] private MovementSystem _movementSystem;
    [SerializeField] private GravityPower _gravityPower;
    [SerializeField] private Jetpack _jetpack;
    [SerializeField] private InteractiveRayCast _interactiveRaycast;
    [SerializeField] private PlayerAnimationController _animController;

    [SerializeField] private Image dialogueBox;
    [SerializeField] private Text textArea;   
    [SerializeField] private Text nameTextArea;

    [SerializeField] private GameObject _continueBox;

    private bool onDialogue = false;
    private bool onWritingSentence = false;
    private bool onWritingName = false;
    private bool forcedDisplay = false;
    private bool couldMove = false;
    private bool couldUseGravity = false;
    private bool couldUseJetpack = false;
    private bool couldInteract = false;

    private string _currentDialogue;
    private string _currentNameDialogue;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    /* This method is called when something wants to start a dialogue
     * The manager will start it only if the player is not onDialogue already or if he is falling
     */
    public void StartDialogue (Dialogue dialogue, DialogueTrigger trigger)
    {
        if((!onDialogue && !trigger.skippable && !trigger.destroyable)  || (!onDialogue && _movementSystem.IsLanded()))
        {
            _movementSystem.canControl = false;
            InitializeDialogue(dialogue, trigger);
            DisplayNextSentence();
        }
    }

    /*When a dialogue starts the player can only move the his view, he cannot move or use the gravity poers
     */
    private void InitializeDialogue(Dialogue dialogue, DialogueTrigger trigger)
    {
        _currentTrigger = trigger;

        //A Dialog will prevent the player from moving and using gravity power. If an object is attracted i will be immediatly released
        //couldMove = _movementSystem.enabled;
        couldUseGravity = _gravityPower.enabled;
        couldUseJetpack = _jetpack.enabled;
        couldInteract = _interactiveRaycast.enabled;
        //_movementSystem.enabled = false;
        _gravityPower.ForceRealease();
        _gravityPower.enabled = false;
        _jetpack.enabled = false;
        _interactiveRaycast.enabled = false;

        _animController.DialogEvent();

        dialogueBox.gameObject.SetActive(true);
        onDialogue = true;
        trigger.Started();

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
    }

    /* This method will display the next dialogue sentence 
     */
    public void DisplayNextSentence()
    {
        _continueBox.SetActive(false);

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        string[] words = sentence.Split(':');

        _currentNameDialogue = words[0]; //Load the name if the player wants it to show all in one
        _currentDialogue = words[1]; //Load the dialog if the player wants it to show all in one

        onWritingSentence = true;
        if (!nameTextArea.text.Equals(words[0]))
        {
            onWritingName = true;
            StartCoroutine(TypeName(words[0]));
        }
        StartCoroutine(TypeSentence(words[1])); 
    }

    /*The following coroutine will display the current sentence using an animation that display a letter per frame
     */
    IEnumerator TypeSentence (string sentence)
    {
        textArea.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            textArea.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        _continueBox.SetActive(true);
        onWritingSentence = false;
    }
    
    IEnumerator TypeName (string sentence)
    {
        nameTextArea.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            nameTextArea.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        onWritingName = false;
    }

    private void DisplayAllSentence()
    {
        textArea.text = _currentDialogue;
        nameTextArea.text = _currentNameDialogue;
    }

    void EndDialogue()
    {
        if(couldMove)
        {
            //_movementSystem.enabled = true;
            couldMove = false;
        }
        if(couldUseGravity)
        {
            _gravityPower.enabled = true;
            couldUseGravity = false;
        }
        if(couldUseJetpack)
        {
            _jetpack.enabled = true;
            couldUseJetpack = false;
        }
        if(couldInteract)
        {
            _interactiveRaycast.enabled = true;
            couldInteract = false;
        }

        dialogueBox.gameObject.SetActive(false);
        textArea.text = "";
        nameTextArea.text = "";
        onDialogue = false;
        _currentTrigger.EndDialogue();

        _movementSystem.canControl = true;

        _currentTrigger = null;
    }

    private void LateUpdate()
    {
        if (onDialogue && (onWritingSentence || onWritingName) && Input.GetKeyDown(KeyCode.Space) && !GameEvent.isPaused)
        {
            StopAllCoroutines();
            DisplayAllSentence();

            _continueBox.SetActive(true);
            onWritingSentence = false;
            onWritingName = false;
        }

        else if (onDialogue && !onWritingSentence && !onWritingName && Input.GetKeyDown(KeyCode.Space) && !GameEvent.isPaused) DisplayNextSentence();
    }
}
