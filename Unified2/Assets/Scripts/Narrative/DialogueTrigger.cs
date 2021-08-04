using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private DialogueManager dm;

    public bool started { get; private set; }
    public bool finished { get; private set; }

    public bool skippable;
    public bool destroyable;

    private void Start()
    {
        dm = FindObjectOfType<DialogueManager>();
        started = false;
        finished = false;
    }

    public void TriggerDialogue()
    {
        dm.StartDialogue(dialogue, this);

        BoxCollider collider = GetComponent<BoxCollider>();
        if (!started)
        {
            if (!skippable && !destroyable)
            {
                StartCoroutine(RetriggerDialogue());
            }
        }
        else if (collider != null)
        {
            if (collider.isTrigger)
            {
                collider.enabled = false;
            }
            else
            {
                Destroy(this);
            }
        }
    }

    public void TriggerDialogueDelayed(float delay)
    {
        StartCoroutine(Delay(delay));
    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);

        TriggerDialogue();
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

    private IEnumerator RetriggerDialogue()
    {
        while (!started)
        {
            TriggerDialogue();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>() && !started)
        {
            TriggerDialogue();
        }
            
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>() && !started)
        {
            TriggerDialogue();
        }
    }
}
