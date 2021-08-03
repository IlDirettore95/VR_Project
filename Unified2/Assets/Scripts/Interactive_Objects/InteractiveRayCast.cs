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
        if(_gravityPower.enabled && _gravityPower.IsAttracting())//rado al suolo tutto
        {
            InteractionGeneric.enabled = false;
            InteractionKey.enabled = false;
            InteractionBox_attract.enabled = false;
            InteractionBox_throw.enabled = true;
            
        }
        else
        {
            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;
        
            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.collider.gameObject;
                //Debug.Log("sparo raggio");
                
                if (go.GetComponent<Switch>() && hit.distance <= interactionRange)
                {
                    Debug.Log("ho colliso con un interruttore");
                    InteractionKey.enabled = true;
                    InteractionBox_throw.enabled = false;
                    InteractionBox_attract.enabled = false;
                    InteractionGeneric.enabled = false;
                    Debug.Log("cristo");
                    Switch sw = hit.collider.GetComponent<Switch>();
                
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        sw.commutationDelayed(0.6f);             
                        _animController.Interact();
                        StartCoroutine(StopInteraction());

                        go.GetComponent<AudioSource>().PlayDelayed(0.6f);
                    }
                    
                }
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

                //If this go has a dialogue trigger i start it
                DialogueTrigger _dialogueTrigger = go.GetComponent<DialogueTrigger>();

                if (_dialogueTrigger != null && _dialogueTrigger.enabled && hit.distance <= interactionRange)
                {                   
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        go.GetComponent<DialogueTrigger>().TriggerDialogueDelayed(0.6f);
                    }       
                }
                
            
                else if (hit.distance <= attractionRange && _gravityPower.enabled && (go.GetComponentInParent<ReactiveBox>() || go.GetComponentInParent<Enemy>() || go.GetComponentInParent<ReactiveFan>() || go.GetComponentInParent<ReactiveGrid>()))//caso inter. cassa
                {
                   InteractionBox_attract.enabled = true;//gestire immagini
                   InteractionKey.enabled = false;
                   InteractionBox_throw.enabled = false;
                   InteractionGeneric.enabled = false;
                }
                
                else 
                {
                    Debug.Log("diocristo");
                    InteractionKey.enabled = false;
                    InteractionBox_throw.enabled = false;
                    InteractionBox_attract.enabled = false;
                    InteractionGeneric.enabled = false;
                }
            }
            
            else//rado al suolo tutto
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
