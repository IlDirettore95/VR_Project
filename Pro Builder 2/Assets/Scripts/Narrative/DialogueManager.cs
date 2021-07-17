using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private DialogueTrigger _currentTrigger;

    [SerializeField] private MovementSystem _movementSystem;
    [SerializeField] private GravityPower _gravityPower;
    [SerializeField] private Jetpack _jetpack;
    [SerializeField] private InteractiveRayCast _interactiveRaycast;

    [SerializeField] private Image dialogueBox;
    [SerializeField] private Text textArea;   
    [SerializeField] private Text nameTextArea;
    private bool onDialogue = false;
    private bool onWritingSentence = false;
    private bool onWritingName = false;
    private bool couldMove = false;
    private bool couldUseGravity = false;
    private bool couldUseJetpack = false;
    private bool couldInteract = false;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue (Dialogue dialogue, DialogueTrigger trigger)
    {
        if(!onDialogue && _movementSystem.IsLanded())
        {
            _currentTrigger = trigger;

            //A Dialog will prevent the player from moving and using gravity power. If an object is attracted i will be immediatly released
            couldMove = _movementSystem.enabled;
            couldUseGravity = _gravityPower.enabled;
            couldUseJetpack = _jetpack.enabled;
            couldInteract = _interactiveRaycast.enabled;
            _movementSystem.enabled = false;
            _gravityPower.ForceRealease();
            _gravityPower.enabled = false;
            _jetpack.enabled = false;
            _interactiveRaycast.enabled = false;

            dialogueBox.gameObject.SetActive(true);
            onDialogue = true;
            sentences.Clear();

            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }
        
    }

    public void DisplayNextSentence()
    {
        if( sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        string[] words = sentence.Split(':');
        
        onWritingSentence = true;
        if (!nameTextArea.text.Equals(words[0]))
        {
            onWritingName = true;
            StartCoroutine(TypeName(words[0]));
        }
        StartCoroutine(TypeSentence(words[1]));
    }

    IEnumerator TypeSentence (string sentence)
    {
        textArea.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            textArea.text += letter;
            yield return new WaitForSeconds(0.01f);
        }

        onWritingSentence = false;
    }
    
    IEnumerator TypeName (string sentence)
    {
        nameTextArea.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            nameTextArea.text += letter;
            yield return new WaitForSeconds(0.01f);
        }

        onWritingName = false;
    }

    void EndDialogue()
    {
        if(couldMove)
        {
            _movementSystem.enabled = true;
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
        _currentTrigger = null;
    }

    private void Update()
    {
        if(onDialogue && !onWritingSentence && !onWritingName && Input.GetKeyDown(KeyCode.Return)) DisplayNextSentence();
    }
}
