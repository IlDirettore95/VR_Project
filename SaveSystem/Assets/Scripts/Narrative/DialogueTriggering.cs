using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggering : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;
    private bool started = false;

    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerStatus>() && !started)
        {
            started = true;
            StartCoroutine(TriggerDialogue());   
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>() && !started)
        {
            started = true;
            StartCoroutine(TriggerDialogue());
        }
    }

    private IEnumerator TriggerDialogue()
    {
        while(!dialogueTrigger.started)
        {
            yield return new WaitForSeconds(0.5f);
            dialogueTrigger.TriggerDialogue();
        }    
        gameObject.SetActive(false);
    }
}
