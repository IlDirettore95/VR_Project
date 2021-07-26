using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*A dialogue might start after a collision with a trigger
 * If the dialogue trigger is skippable this script will try to start it once.
 * If the dialogue trigger is destroyable this script will destroy his gameobject if it tries to start and doesn't start immediatly
 */
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
            if(!dialogueTrigger.IsSkippable()) StartCoroutine(TriggerDialogue());
            else dialogueTrigger.TriggerDialogue();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>() && !started)
        {
            started = true;
            if (!dialogueTrigger.IsSkippable()) StartCoroutine(TriggerDialogue());
            else dialogueTrigger.TriggerDialogue();
        }
    }

    private IEnumerator TriggerDialogue()
    {
        while(!dialogueTrigger.started)
        {     
            dialogueTrigger.TriggerDialogue();
            if(!!dialogueTrigger.started && dialogueTrigger.IsDestroyable()) Destroy(gameObject);
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(gameObject);
    }
}
