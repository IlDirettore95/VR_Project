using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*A dialogue might start after a collision with a trigger
 * If the dialogue trigger is skippable this script will try to start it once.
 * If the dialogue trigger is destroyable this script will destroy (set unactive) his gameobject if it tries to start and doesn't start immediatly
 * A dialogue if destroyable must be skippable too
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
        TryToStartDialogue(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryToStartDialogue(other);
    }

    private void TryToStartDialogue(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>() && !started)
        {
            started = true;

            //Is it destroyable or skippable
            if (dialogueTrigger.IsSkippable() || dialogueTrigger.IsDestroyable())
            {
                dialogueTrigger.TriggerDialogue();
                if (!dialogueTrigger.started)
                {
                    if (dialogueTrigger.IsSkippable()) started = false;
                    else gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                StartCoroutine(TriggerDialogue());
            }
        }
    }

    private IEnumerator TriggerDialogue()
    {
        while(!dialogueTrigger.started)
        {     
            dialogueTrigger.TriggerDialogue();
            yield return new WaitForSeconds(0.5f);
        }
        gameObject.SetActive(false);
    }
}
