using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    [SerializeField] private Text textArea;
    [SerializeField] private Image dialogueBox;
    [SerializeField] private Text nameTextArea;
    private Boolean onDialog = false;
    private Boolean onWriting = false;
    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue (Dialogue dialogue)
    {
        //Debug.Log("Starting conversation with " + dialogue.name);
        dialogueBox.enabled = true;
        onDialog = true;
        sentences.Clear();

        foreach( string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
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
        //StopAllCoroutines(); //Stop other text animation if the player choose to start a new one.
        onWriting = true;
        
        StartCoroutine(TypeName(words[0]));
        StartCoroutine(TypeSentence(words[1]));
    }

    IEnumerator TypeSentence (string sentence)
    {
        textArea.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            textArea.text += letter;
            //Debug.Log(currentSentence);
            yield return new WaitForSeconds(0.05f);
        }

        onWriting = false;
    }
    
    IEnumerator TypeName (string sentence)
    {
        nameTextArea.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            nameTextArea.text += letter;
            //Debug.Log(currentSentence);
            yield return new WaitForSeconds(0.05f);
        }

        //onWriting = false;
    }

    void EndDialogue()
    {
        dialogueBox.enabled = false;
        textArea.text = "";
        nameTextArea.text = "";
        onDialog = false;
        //Debug.Log("End conversation");
    }

    private void Update()
    {
        if(onDialog && !onWriting && Input.GetKeyDown(KeyCode.Return)) DisplayNextSentence();
    }
}
