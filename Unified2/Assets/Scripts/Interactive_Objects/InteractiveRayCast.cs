using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class InteractiveRayCast : MonoBehaviour
{
    private Camera _camera;
    public float attractionRange;
    public float interactionRange;
    public Image InteractionKey;
    public Image InteractionBox_throw;
    public Image InteractionBox_attract;
    public Image InteractionGeneric;

    private GravityPower _gravityPower;

    //Animation
    PlayerAnimationController _animController;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _gravityPower = GetComponent<GravityPower>();
        _animController = GetComponentInParent<PlayerAnimationController>();
    }

    private void OnDisable()
    {
        InteractionKey.enabled = false;
        InteractionBox_throw.enabled = false;
        InteractionBox_attract.enabled = false;
        InteractionGeneric.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        * hud image managing, this is attraction case so you can see
        * throw and release hud images, everything else must be disabled.
        * Note that image enabling on screen is mutually exclusive,
        * only one image could be enabled at once.
        * It doesn't need to check raycasting at this stage because 
        * we are checking gravityPower's booleans.
        */
        if(_gravityPower.enabled && _gravityPower.IsAttracting())
        {
            InteractionGeneric.enabled = false;
            InteractionKey.enabled = false;
            InteractionBox_attract.enabled = false;
            InteractionBox_throw.enabled = true;
            
        }
        //this is the case of raycasting check within a certain range, starting from the camera point.
        else
        {
            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;
        
            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.collider.gameObject;
                
                //switch collision check, the switch sound and animation are fine tuned with coroutines.
                if (go.GetComponent<Switch>() && hit.distance <= interactionRange)
                {
                    InteractionKey.enabled = true;
                    InteractionBox_throw.enabled = false;
                    InteractionBox_attract.enabled = false;
                    InteractionGeneric.enabled = false;
                    Switch sw = hit.collider.GetComponent<Switch>();
                
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        sw.commutationDelayed(0.6f);             
                        _animController.Interact();
                        StartCoroutine(StopInteraction());

                        go.GetComponent<AudioSource>().PlayDelayed(0.35f);
                    }
                    
                }
                //terminal collision check, the same as switch.
                else if(go.tag.Equals("Terminal") && hit.distance <= interactionRange)
                {
                    InteractionGeneric.enabled = true;
                    InteractionBox_throw.enabled = false;
                    InteractionBox_attract.enabled = false;
                    InteractionKey.enabled = false;

                    if (Input.GetKeyDown(KeyCode.E))
                    {                       
                        _animController.Interact();
                        StartCoroutine(StopInteraction());

                        go.GetComponent<AudioSource>().PlayDelayed(0.6f);
                    }
                }
                //this is the case of raycasting hitting some grabbable obj that can have one of theese component attached.
                else if (hit.distance <= attractionRange && _gravityPower.enabled && (go.GetComponentInParent<ReactiveBox>() || go.GetComponentInParent<Enemy>() || go.GetComponentInParent<ReactiveFan>() || go.GetComponentInParent<ReactiveGrid>()))
                {
                   InteractionBox_attract.enabled = true; //inform the player that he can grab the obj on screen.
                   InteractionKey.enabled = false;
                   InteractionBox_throw.enabled = false;
                   InteractionGeneric.enabled = false;
                }
                //exit case to disable the images.
                else 
                {
                    InteractionKey.enabled = false;
                    InteractionBox_throw.enabled = false;
                    InteractionBox_attract.enabled = false;
                    InteractionGeneric.enabled = false;
                }

                //If this go has a dialogue trigger i start it
                DialogueTrigger _dialogueTrigger = go.GetComponent<DialogueTrigger>();

                if (_dialogueTrigger != null && _dialogueTrigger.enabled && hit.distance <= interactionRange)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        go.GetComponent<DialogueTrigger>().TriggerDialogueDelayed(0.6f);
                    }
                }
            }
            /*
             * this is the case where you pass with the mouse from an interactable obj
             * to none of them. Must disable all images on screen.
             */
            else
            {
            
                InteractionKey.enabled = false;
                InteractionBox_throw.enabled = false;
                InteractionBox_attract.enabled = false;
                InteractionGeneric.enabled = false;
            } 
        }
    }

    private IEnumerator StopInteraction()
    {
        yield return null;

        _animController.StopInteraction();
    }
}
