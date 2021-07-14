using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class interactiveRayCast : MonoBehaviour
{
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject go = hit.collider.gameObject;
            //Debug.Log("sparo raggio");
            if (go.GetComponent<Switch>())
            {
                Debug.Log("ho colliso con un interruttore");
                Switch sw = hit.collider.GetComponent<Switch>();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    sw.commutation();
                   
                }
                    
            }
        } 
            
        
    }
}
