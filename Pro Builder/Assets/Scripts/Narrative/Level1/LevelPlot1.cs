using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/*This class handles level 1's objectives (plot)
 * Every state rappresent an objective:
 *      Start: setting
 *      Introduction: first dialogue
 *      Tutorial1: the player must walk
 *      Tutorial2: the player must jump
 *      Tutorial3: the player must use gravity powers
 *      TryToExit: in this state the player is free to find a way to exit
 *      EscapeFormRoom: the player must find a way to escape from the first room
 *      Tutorial4: The player must run
 *      Explore: The player should explore
 *      Jetpack: the player must recover the jetpack
 *      UseJetpack: the player must use the jetpack to reach an upper corridor
 *      FindAnExit: the player must find an exit
 */
public class LevelPlot1 : MonoBehaviour
{
    private LevelState1 _currentState; //Current state of the plot state machine

    private DialogueTrigger[] dialogues; //List of dialogues plot will trigger

    [SerializeField] private MovementSystem _movementSystem;
    [SerializeField] private Jetpack _jetpack;
    [SerializeField] private GravityPower _gravityPower;

    [SerializeField] private GameObject _lockedDoor_1;
    private DialogueTrigger _lockedDoor1Dialogue;

    [SerializeField] private GameObject _corridor;
    private DialogueTrigger _corridorDialogue;

    [SerializeField] private GameObject _lockedDoor_2;
    private DialogueTrigger _lockedDoor2Dialogue;

    [SerializeField] private GameObject _failedJump;
    private DialogueTrigger _failedJumpDialogue;

    [SerializeField] private GameObject _jetpackStation;
    private DialogueTrigger _jetpackStationDialogue;

    //Objectives
    [SerializeField] private string[] objectives;

    //Booleans and vars
    private float objectiveDelay = 1f; //The goal will be considered reached after a delay
    private float nextTimeObjective;
    private bool objectiveDone = false;
    //--------------------------------------------------------------------------------------------------------------------------------
    private bool isWalking = false; //During Tutorial1, this boolean will track if the player is walking or not
    private float timeGoalWalking = 1.5f; //Amount of time the player must walk
    private float nextTimeWalking; //Istant on which the player can stop walking
    //--------------------------------------------------------------------------------------------------------------------------------
    private bool hasJumped = false;
    //--------------------------------------------------------------------------------------------------------------------------------
    private bool usedGravityPower = false;
    //--------------------------------------------------------------------------------------------------------------------------------
    private bool isRunning = false; //During Tutorial4, this boolean will track if the player is running or not
    private float timeGoalRunning = 2f; //Amount of time the player must run
    private float nextTimeRunning; //Istant on which the player can stop running
    //--------------------------------------------------------------------------------------------------------------------------------

    //Graphics
    [SerializeField] GameObject _popUpObjective;
    [SerializeField] Text _objectiveText;

    // Start is called before the first frame update
    void Start()
    {
        dialogues = GetComponents<DialogueTrigger>();
        _jetpack.enabled = false; //The main character starts with no jetpack
        _gravityPower.enabled = false;   
        _lockedDoor_1.SetActive(false);
        _lockedDoor_2.SetActive(false);
        _failedJump.SetActive(false);
        _lockedDoor1Dialogue = _lockedDoor_1.GetComponent<DialogueTrigger>();
        _corridorDialogue = _corridor.GetComponent<DialogueTrigger>();
        _lockedDoor2Dialogue = _lockedDoor_2.GetComponent<DialogueTrigger>();
        _jetpackStationDialogue = _jetpackStation.GetComponent<DialogueTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(_currentState)
        {
            case LevelState1.Start:          
                if(_movementSystem.isGrounded)
                {
                    dialogues[0].TriggerDialogue();
                    _movementSystem.enabled = false;
                    _currentState = LevelState1.Introduction;
                }                      
                break;

            case LevelState1.Introduction:
                if(dialogues[0].finished)
                {
                    _movementSystem.enabled = true;
                    DisplayObjective(objectives[0]);
                    _currentState = LevelState1.Tutorial1;
                }
                break;

            case LevelState1.Tutorial1:
                if(objectiveDone)
                {
                    if (dialogues[1].finished)
                    {
                        objectiveDone = false;
                        DisplayObjective(objectives[1]);
                        _currentState = LevelState1.Tutorial2;
                    }
                    else if (!dialogues[1].started && Time.time > nextTimeObjective)
                    {            
                        dialogues[1].TriggerDialogue();
                    }
                }   
                else if(!isWalking && _movementSystem.walking )
                {
                    isWalking = true;
                    nextTimeWalking = Time.time + timeGoalWalking;
                }
                else if(isWalking && _movementSystem.walking && Time.time > nextTimeWalking)
                {
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                else if(!_movementSystem.walking)
                {
                    isWalking = false;
                }
                break;

            case LevelState1.Tutorial2:
                if(objectiveDone)
                {
                    if (dialogues[2].finished)
                    {
                        _gravityPower.enabled = true;
                        objectiveDone = false;
                        DisplayObjective(objectives[2]);
                        _currentState = LevelState1.Tutorial3;
                    }
                    else if(!dialogues[2].started && Time.time > nextTimeObjective)
                    {
                        dialogues[2].TriggerDialogue();
                    }
                    
                }             
                else if(_movementSystem.hasJumped)
                {    
                    hasJumped = true;  
                }
                else if(_movementSystem.isGrounded && hasJumped)
                {
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;                      
                }
                break;

            case LevelState1.Tutorial3:
                if(objectiveDone)
                {
                    if (dialogues[3].finished)
                    {
                        objectiveDone = false;
                        _lockedDoor_1.SetActive(true);
                        DisplayObjective(objectives[3]);
                        _currentState = LevelState1.TryToExit;
                    }
                    else if (!dialogues[3].started && Time.time > nextTimeObjective)
                    {
                        dialogues[3].TriggerDialogue();
                    }
                    
                }    
                else if(usedGravityPower && !_gravityPower.IsAttracting())
                {
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                else if (_gravityPower.IsAttracting())
                {
                    usedGravityPower = true; 
                }   
                break;

            case LevelState1.TryToExit:
                if(_corridorDialogue.finished)
                {
                    _corridor.SetActive(false);
                    _lockedDoor_1.SetActive(false);
                    DisplayObjective(objectives[5]);
                    _currentState = LevelState1.Tutorial4;
                }
                else if(_lockedDoor1Dialogue.finished)
                {
                    _lockedDoor_1.SetActive(false);
                    DisplayObjective(objectives[4]);
                    _currentState = LevelState1.EscapeFromRoom;
                }
                break;

            case LevelState1.EscapeFromRoom:
                if (_corridorDialogue.finished)
                {
                    _corridor.SetActive(false);
                    DisplayObjective(objectives[5]);
                    _currentState = LevelState1.Tutorial4;
                }
                break;

            case LevelState1.Tutorial4:
                if (objectiveDone)
                {
                    if (dialogues[4].finished)
                    {
                        objectiveDone = false;
                        _lockedDoor_2.SetActive(true);
                        _failedJump.SetActive(true);
                        DisplayObjective(objectives[6]);
                        _currentState = LevelState1.Explore;
                    }
                    else if (!dialogues[4].started && Time.time > nextTimeObjective)
                    {
                        dialogues[4].TriggerDialogue();
                    }

                }
                else if (!isRunning && _movementSystem.running)
                {
                    isRunning = true;
                    nextTimeRunning = Time.time + timeGoalRunning;
                }
                else if (isRunning && _movementSystem.running && Time.time > nextTimeRunning)
                {
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                else if (!_movementSystem.running)
                {
                    isRunning = false;
                }
                break;

            case LevelState1.Explore:
                if(_lockedDoor2Dialogue.finished)
                {
                    DisplayObjective(objectives[7]);
                    _currentState = LevelState1.Jetpack;
                }
                else if(_jetpackStationDialogue.finished)
                {
                    _jetpack.enabled = true;
                    DisplayObjective(objectives[8]);
                    _currentState = LevelState1.UseJetpack;
                }
                break;

            case LevelState1.Jetpack:
                if (objectiveDone)
                {
                    if (Time.time > nextTimeObjective)
                    {
                        objectiveDone = false;
                        DisplayObjective(objectives[8]);
                        _currentState = LevelState1.UseJetpack;
                    }

                }
                else if(_jetpackStationDialogue.finished)
                {
                    _jetpack.enabled = true;
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                break;

            case LevelState1.UseJetpack:
                if(objectiveDone)
                {
                    if(Time.time > nextTimeObjective)
                    {
                        objectiveDone = false;
                        DisplayObjective(objectives[9]);
                        _currentState = LevelState1.FindAnExit;
                    }
                }
                else if(_movementSystem.jetpack)
                {
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                break;

            case LevelState1.FindAnExit:
                break;

        }
    }

    private void DisplayObjective(string objective)
    {
        if(objective == null)
        {
            _popUpObjective.SetActive(false);
        }
        else
        {
            _popUpObjective.SetActive(true);
            _objectiveText.text = objective;
        }
    }

    public enum LevelState1
    {
        Start,
        Introduction,
        Tutorial1,
        Tutorial2,
        Tutorial3,
        TryToExit,
        EscapeFromRoom,
        Tutorial4,
        Explore,
        Jetpack,
        UseJetpack,
        FindAnExit
    }
}
