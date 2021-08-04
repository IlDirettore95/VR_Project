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
 *      ExitFromRoom: in this state the player is free to find a way to exit
 *      Explore: The player should explore
 *      RepairJetpack: The player should find to repair his jetack
 *      Jetpack: the player must recover the jetpack
 *      FindAnExit: the player must find an exit
 */
public class LevelPlot1 : MonoBehaviour
{
    private GameObject _player;

    private LevelState1 _currentState; //Current state of the plot state machine

    private DialogueTrigger[] dialogues; //List of dialogues plot will trigger

    private MovementSystem _movementSystem;
    private Jetpack _jetpack;
    private GravityPower _gravityPower;
    private Overlay _statusOverlay;

    [SerializeField] private GameObject _grid;

    [SerializeField] private GameObject _lockedDoor_1;
    private DialogueTrigger _lockedDoor1Dialogue;

    [SerializeField] private GameObject _airCondition;
    private DialogueTrigger _airConditionDialogue;

    [SerializeField] private GameObject _corridor;
    private DialogueTrigger _corridorDialogue;

    [SerializeField] private GameObject _lockedDoor_2;
    private DialogueTrigger _lockedDoor2Dialogue;

    [SerializeField] private GameObject _failedJump;
    private DialogueTrigger _failedJumpDialogue;

    [SerializeField] private GameObject _jumpSucceded;
    private DialogueTrigger _jumpSuccededDialogue;

    [SerializeField] private GameObject _jetpackRoom;
    private DialogueTrigger _jetpackRoomDialogue;

    [SerializeField] private GameObject _jetpackStation;
    private DialogueTrigger _jetpackStationDialogue;
    private BoxCollider _jetpackStationCollider;

    [SerializeField] private GameObject _jetpackSucceded;
    private DialogueTrigger _jetpackSuccededDialogue;

    [SerializeField] private GameObject _endOfLevel;
    [SerializeField] private GameObject _exitDoor;
    private UnlockableDoorAnimation _exitDoorAnimation;

    //Objectives
    [SerializeField] private string[] objectives;

    //Booleans and vars
    private float objectiveDelay = 0.5f; //The goal will be considered reached after a delay
    private float nextTimeObjective;
    private bool objectiveDone = false;
    //--------------------------------------------------------------------------------------------------------------------------------
    private bool wasWalking = false; //During Tutorial1, this boolean will track if the player is walking or not
    private float timeGoalWalking = 1.5f; //Amount of time the player must walk
    private float nextTimeWalking; //Istant on which the player can stop walking
    //--------------------------------------------------------------------------------------------------------------------------------
    private bool hasJumped = false;
    //--------------------------------------------------------------------------------------------------------------------------------
    private bool usedGravityPower = false;
    //--------------------------------------------------------------------------------------------------------------------------------
    private Vector3 _gridStartPosition;

    //Graphics
    private ObjectiveManager _objectiveManager;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _objectiveManager = GameObject.FindObjectOfType<ObjectiveManager>();

        _movementSystem = _player.GetComponent<MovementSystem>();
        _jetpack = _player.GetComponent<Jetpack>();
        _gravityPower = _player.GetComponentInChildren<GravityPower>();
        _statusOverlay = _player.GetComponentInChildren<Overlay>();

        dialogues = GetComponents<DialogueTrigger>();
        _gravityPower.enabled = false;   

        _lockedDoor_1.SetActive(false);
        _lockedDoor_2.SetActive(false);
        _jumpSucceded.SetActive(false);
        _airCondition.SetActive(false);
        _failedJump.SetActive(false);
        _corridor.SetActive(false);
        _jetpackRoom.SetActive(false);
        _jetpackSucceded.SetActive(false);

        _endOfLevel.SetActive(false);

        _lockedDoor1Dialogue = _lockedDoor_1.GetComponent<DialogueTrigger>();
        _airConditionDialogue = _airCondition.GetComponent<DialogueTrigger>();
        _corridorDialogue = _corridor.GetComponent<DialogueTrigger>();
        _lockedDoor2Dialogue = _lockedDoor_2.GetComponent<DialogueTrigger>();
        _jetpackStationDialogue = _jetpackStation.GetComponent<DialogueTrigger>();
        _jetpackRoomDialogue = _jetpackRoom.GetComponent<DialogueTrigger>();
        _failedJumpDialogue = _failedJump.GetComponent<DialogueTrigger>();
        _jumpSuccededDialogue = _jumpSucceded.GetComponent<DialogueTrigger>();
        _jetpackSuccededDialogue = _jetpackSucceded.GetComponent<DialogueTrigger>();

        _exitDoorAnimation = _exitDoor.GetComponentInChildren<UnlockableDoorAnimation>();
        _exitDoorAnimation.LockDoor();

        _statusOverlay.ActiveBar(2, false);
        _statusOverlay.ActiveBar(3, false);

        _gridStartPosition = _grid.transform.position;

        _jetpackStationCollider = _jetpackStation.GetComponent<BoxCollider>();

        _jetpackStationCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch(_currentState)
        {
            case LevelState1.Start:
                if (!_movementSystem.enabled)
                {
                    _movementSystem.enabled = true;
                }
                else if(_movementSystem.IsLanded())
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
                    _objectiveManager.DisplayObjective(objectives[0]);
                    _currentState = LevelState1.Tutorial1;
                }
                break;

            case LevelState1.Tutorial1:
                if(objectiveDone)
                {
                    if (dialogues[1].finished)
                    {
                        objectiveDone = false;
                        _objectiveManager.DisplayObjective(objectives[1]);
                        _currentState = LevelState1.Tutorial2;
                    }
                    else if (!dialogues[1].started && Time.time > nextTimeObjective)
                    {            
                        dialogues[1].TriggerDialogue();
                    }
                }   
                else if(!wasWalking && _movementSystem.walking)
                {
                    wasWalking = true;
                    nextTimeWalking = Time.time + timeGoalWalking;
                }
                else if(wasWalking && _movementSystem.walking && Time.time > nextTimeWalking)
                {
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                else if(!_movementSystem.walking)
                {
                    wasWalking = false;
                }
                break;

            case LevelState1.Tutorial2:
                if(objectiveDone)
                {
                    if (dialogues[2].finished)
                    {
                        _gravityPower.enabled = true;
                        _statusOverlay.ActiveBar(2, true);
                        objectiveDone = false;
                        _objectiveManager.DisplayObjective(objectives[2]);
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

                if (_airCondition != null && !_grid.transform.position.Equals(_gridStartPosition) && !_airConditionDialogue.started && !_airConditionDialogue.finished) _airCondition.SetActive(true);

                if (objectiveDone)
                {
                    if (dialogues[3].finished)
                    {
                        objectiveDone = false;
                        _lockedDoor_1.SetActive(true);
                        _corridor.SetActive(true);
                        _objectiveManager.DisplayObjective(objectives[3]);
                        _currentState = LevelState1.ExitFromRoom;
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

            case LevelState1.ExitFromRoom:
                if (_airCondition != null && !_grid.transform.position.Equals(_gridStartPosition) && !_airConditionDialogue.started && !_airConditionDialogue.finished) _airCondition.SetActive(true);

                if (_corridorDialogue.finished)
                {
                    _lockedDoor_1.SetActive(false);
                    _failedJump.SetActive(true);
                    _jumpSucceded.SetActive(true);
                    _lockedDoor_2.SetActive(true);
                    _objectiveManager.DisplayObjective(objectives[5]);
                    _currentState = LevelState1.Explore;
                }
                else if(_lockedDoor1Dialogue.finished)
                {
                    _objectiveManager.DisplayObjective(objectives[4]);
                }
                break;

            case LevelState1.Explore:
                if(_jetpack.enabled)
                {
                    _jetpackSucceded.SetActive(true);

                    if (_jetpackSuccededDialogue.finished || _lockedDoor2Dialogue.finished)
                    {
                        _objectiveManager.DisplayObjective(objectives[8]);
                        _currentState = LevelState1.FindAnExit;
                    }
                }
                else if(!_jetpack.enabled)
                {
                    if (_jetpackRoomDialogue.finished)
                    {
                        _jetpackStationCollider.enabled = true;
                        _objectiveManager.DisplayObjective(objectives[7]);
                        _currentState = LevelState1.Jetpack;
                    }
                    else if (_lockedDoor2Dialogue.finished)
                    {
                        _objectiveManager.DisplayObjective(objectives[6]);
                        _currentState = LevelState1.RepairJetpack;
                    }
                    else if (_jumpSuccededDialogue.finished)
                    {
                        _failedJump.SetActive(false);
                        if(!_jetpackRoomDialogue.started && !_jetpackRoomDialogue.finished)_jetpackRoom.SetActive(true);
                    }
                }
                break;

            case LevelState1.RepairJetpack:
                if (_jetpackRoomDialogue.finished)
                {
                    _jetpackStationCollider.enabled = true;
                    _objectiveManager.DisplayObjective(objectives[7]);
                    _currentState = LevelState1.Jetpack;
                }
                else if (_jumpSuccededDialogue.finished)
                {
                    _failedJump.SetActive(false);
                    _jetpackRoom.SetActive(true);
                }
                break;

            case LevelState1.Jetpack:
                if (objectiveDone)
                {
                    if (Time.time > nextTimeObjective)
                    {
                        objectiveDone = false;
                        _endOfLevel.SetActive(true);

                        if (_lockedDoor2Dialogue.finished)
                        {
                            _jetpackSucceded.SetActive(true);
                            _objectiveManager.DisplayObjective(objectives[8]);
                            _currentState = LevelState1.FindAnExit;
                        }
                        else
                        {
                            _objectiveManager.DisplayObjective(objectives[5]);
                            _currentState = LevelState1.Explore;
                        }   
                    }
                }
                else if(dialogues[4].finished)
                {
                    _jetpack.enabled = true;
                    _statusOverlay.ActiveBar(3, true);

                    _exitDoorAnimation.UnLockDoor();

                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                else if(_jetpackStationDialogue.finished)
                {
                    dialogues[4].TriggerDialogue();
                }
                else if(_jetpackRoomDialogue.finished && !_jetpackStationDialogue.started)
                {
                    _jetpackStationCollider.enabled = true;
                }
                break;

            case LevelState1.FindAnExit:
                if(_jetpackSuccededDialogue.finished)
                { 
                    enabled = false;
                }
                break;

        }
    }

    public enum LevelState1
    {
        Start,
        Introduction,
        Tutorial1,
        Tutorial2,
        Tutorial3,
        ExitFromRoom,
        Explore,
        RepairJetpack,
        Jetpack,
        FindAnExit
    }
}
