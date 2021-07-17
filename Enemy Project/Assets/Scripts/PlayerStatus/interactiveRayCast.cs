using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class interactiveRayCast : MonoBehaviour
{
    private Camera _camera;
    public float maxDistance;
    public Image InteractionKey;
    public Image InteractionBox_throw;
    public Image InteractionBox_attract;
    public Image InteractionGeneric;

    private GravityPower gravityPower;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        gravityPower = GetComponent<GravityPower>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gravityPower.IsAttracting())//rado al suolo tutto
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
        
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                GameObject go = hit.collider.gameObject;
                Debug.Log("sparo raggio");
                if (go.GetComponent<Switch>())
                {
                    Debug.Log("ho colliso con un interruttore");
                    InteractionKey.enabled = true;
                    InteractionBox_throw.enabled = false;
                    InteractionBox_attract.enabled = false;
                    InteractionGeneric.enabled = false;
                    Switch sw = hit.collider.GetComponent<Switch>();
                
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        sw.commutation();
                   
                    }
                    
                }
                
                else if (go.GetComponent<DialogueTrigger>())
                {
                    InteractionGeneric.enabled = true;
                    InteractionBox_throw.enabled = false;
                    InteractionBox_attract.enabled = false;
                    InteractionKey.enabled = false;
                    
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        go.GetComponent<DialogueTrigger>().TriggerDialogue();
                        Destroy(go.GetComponent<DialogueTrigger>());

                    }
                   
                }
            
                else if (go.GetComponent<ReactiveBox>() || go.GetComponentInParent<Enemy>() || go.GetComponentInParent<ReactiveFan>())//caso inter. cassa
                {
                   InteractionBox_attract.enabled = true;//gestire immagini
                   InteractionKey.enabled = false;
                   InteractionBox_throw.enabled = false;
                   InteractionGeneric.enabled = false;
                }
                
                else 
                {
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
}
