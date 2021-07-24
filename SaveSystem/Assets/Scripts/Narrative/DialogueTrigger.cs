using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private DialogueManager dm;

    public bool started { get; private set;}
    public bool finished { get; private set;}

    private void Start()
    {
        dm = FindObjectOfType<DialogueManager>();
        started = false;
        finished = false;
    }

    public void TriggerDialogue()
    {
        dm.StartDialogue(dialogue, this);
    }

    public void Started()
    {
        started = true;
    }

    public void EndDialogue()
    {
        started = false;
        finished = true;
    }
}
