using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour, InteractableObject
{
    public float maxElevation = 10f;

    public float maxDescent = 10f;

    private float currentElevation = 0f;

    private float currentDescent = 0f;

    private float instantElevation = 0f;

    private float instantDescent = 0f;

    private bool onEnter = false;

    private bool onExit = false;

    private bool isEnabled = false;

    private bool railingIsUp = false;

    public float speed = 1f;

    public GameObject railing;

    private Vector3 currentPosition;
    private Vector3 targetPosition;

    private Vector3 currentPosition1;
    private Vector3 targetPosition1;
    
    private Vector3 currentPosition2;
    private Vector3 targetPosition2;
    

    public float buildUp = 0.1f;
    private float buildUpState = 0;

    public Switch sw;
    
    // Start is called before the first frame update
    void Start()
    {
        currentPosition = railing.transform.position;
        targetPosition = railing.transform.position + new Vector3(0, 1.3f, 0);

        currentPosition1 = targetPosition + new Vector3(0, maxElevation, 0);
        targetPosition1 = currentPosition1 +  new Vector3(0, -1.3f, 0);
        
        currentPosition2 = targetPosition1 + new Vector3(0, 1.3f, 0);
        targetPosition2 = currentPosition  +   new Vector3(0, -1.3f, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    
        if (onEnter && isEnabled)
        {
            
            if (currentElevation == 0) //se l'ascensore è a pian terreno deve fare salire la protezione
            {
                sw.disableSwitch();
                if (buildUpState < 1)//lerping railing on elevation
                {
                    buildUpState += buildUp* Time.deltaTime;
                    Vector3 interpolatedPosition = Vector3.Lerp(currentPosition, targetPosition, buildUpState);
                    railing.transform.position = interpolatedPosition;
                }
                else
                {
                    railingIsUp = true;
                    buildUpState = 0;
                    
                }
            }
            
            //l'ascensore sale
            if (currentElevation < maxElevation && railingIsUp)
            {
                instantElevation = Time.deltaTime * speed;
                currentElevation += instantElevation;
                transform.Translate(0,instantElevation,0,Space.Self);
               
            }
           
             //se l'ascensore è arrivato sopra allora deve far scendere la protezione
            if (currentElevation >= maxElevation)
            {
                
                

                if (buildUpState < 1 && railingIsUp)//lerping railing on elevation
                {
                    buildUpState += buildUp* Time.deltaTime;
                    Vector3 interpolatedPosition = Vector3.Lerp(currentPosition1, targetPosition1, buildUpState);
                    railing.transform.position = interpolatedPosition;
                }

                else
                {
                    railingIsUp = false;
                    buildUpState = 0;
                    sw.enableSwitch();
                }
                
            }

            currentDescent = maxDescent - currentElevation;

        }
        
        else if (onExit && !isEnabled )
        {

            if (currentDescent <= 0)//se l'ascensore è sopra e l'utente azionare l'interruttore allora faccio salire la protezione
            {
                sw.disableSwitch();
                if (buildUpState < 1)//lerping railing on elevation
                {
                    buildUpState += buildUp* Time.deltaTime;
                    Vector3 interpolatedPosition = Vector3.Lerp(targetPosition1, currentPosition2, buildUpState);
                    railing.transform.position = interpolatedPosition;
                }
                
                else
                {
                    railingIsUp = true;
                    buildUpState = 0;
                }
            }
            
            if (currentDescent < maxDescent && railingIsUp)//l'ascensore scende
            {
                instantDescent = Time.deltaTime * speed;
                currentDescent += instantDescent;
                transform.Translate(0,-instantDescent,0,Space.Self);
                

            }

            if (currentDescent >= maxDescent) //se l'ascensore è sceso giu allora faccio scendere la protezione
            {
                if (buildUpState < 1 && railingIsUp)//lerping railing on elevation
                {
                    buildUpState += buildUp* Time.deltaTime;
                    Vector3 interpolatedPosition = Vector3.Lerp(targetPosition, currentPosition, buildUpState);
                    railing.transform.position = interpolatedPosition;
                }
                
                else
                {
                    railingIsUp = false;
                    buildUpState = 0;
                    sw.enableSwitch();
                }
            }

            currentElevation = maxElevation - currentDescent;
            
        }

    }

   

    public void setFalse()
    {
        isEnabled = false;
        onExit = true;
        onEnter = false;
        
    }

    public void setTrue()
    {
        isEnabled = true;
        onEnter = true;
        onExit = false;

        
    }
}
