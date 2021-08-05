using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class handles level 3's objectives (plot)
 * Every state rappresent an objective:
 *      Start: setting
 *      Introduction: first dialogue
 *      AppreachToGarbageShoot
 *      DeactivateTheIncinerator
 *      AccessToIncenerator
 *      ExitFromIncinerator
 */

public class LevelPlot3 : MonoBehaviour
{

    private GameObject _player;

    private LevelState3 _currentState; //Current state of the plot state machine

    private DialogueTrigger[] dialogues; //List of dialogues plot will trigger

    private MovementSystem _movementSystem;
    private Jetpack _jetpack;

    private LevelSystem _levelSys;

    private bool furnaceOn = false; //The furnace is turned off

    [SerializeField] private Switch _switch;

    [SerializeField] private GameObject _garbageChute;
    private DialogueTrigger _garbageChuteDialogue;

    [SerializeField] private GameObject _incenerator;
    private DialogueTrigger _inceneratorDialogue;

    [SerializeField] private GameObject _final;
    private DialogueTrigger _finalDialogue;

    [SerializeField] private GameObject _inceneratorSwitch;
    private DialogueTrigger _inceneratorSwitchDialogue;
    private BoxCollider _inceneratorSwitchCollider;

    [SerializeField] private GameObject _enterDoor;
    private UnlockableDoorAnimation _enterDoorAnimation;


    //Booleans and vars
    private float objectiveDelay = 0.5f; //The goal will be considered reached after a delay
    private float nextTimeObjective;
    private bool objectiveDone = false;

    //Objectives
    [SerializeField] private string[] objectives;

    //Graphics
    private ObjectiveManager _objectiveManager;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        
        _objectiveManager = GameObject.FindObjectOfType<ObjectiveManager>();

        _movementSystem = _player.GetComponent<MovementSystem>();

        _jetpack = _player.GetComponent<Jetpack>();

        _jetpack.enabled = true;

        dialogues = GetComponents<DialogueTrigger>();

        _levelSys = GameObject.FindObjectOfType<LevelSystem>();

        _garbageChute.SetActive(true);

        _garbageChuteDialogue = _garbageChute.GetComponent<DialogueTrigger>();
        _inceneratorDialogue = _incenerator.GetComponent<DialogueTrigger>();
        _finalDialogue = _final.GetComponent<DialogueTrigger>();
        _inceneratorSwitchDialogue = _inceneratorSwitch.GetComponent<DialogueTrigger>();
        _inceneratorSwitchCollider = _inceneratorSwitch.GetComponent<BoxCollider>();
        _inceneratorSwitchCollider.enabled = false;

        _enterDoorAnimation = _enterDoor.GetComponentInChildren<UnlockableDoorAnimation>();
        _enterDoorAnimation.LockDoor();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case LevelState3.Start:
                if (_movementSystem.IsLanded())
                {
                    dialogues[0].TriggerDialogue();
                    _currentState = LevelState3.Introduction;
                }
                break;

            case LevelState3.Introduction:
                if (dialogues[0].finished)
                {
                    _garbageChute.SetActive(true);
                    _objectiveManager.DisplayObjective(objectives[0]);
                    _currentState = LevelState3.ApproachToGarbageShoot;
                }
                break;

            case LevelState3.ApproachToGarbageShoot:

                if (_garbageChuteDialogue.finished)
                {
                    _inceneratorSwitchCollider.enabled = true;
                    _objectiveManager.DisplayObjective(objectives[1]);
                    _currentState = LevelState3.DeactivateTheIncinerator;
                }
                else if(!furnaceOn && _garbageChuteDialogue.started)
                {
                    _switch.commutation();
                    furnaceOn = true;
                }
                break;

            case LevelState3.DeactivateTheIncinerator:
                if (objectiveDone)
                {
                    if (Time.time > nextTimeObjective)
                    {
                        objectiveDone = false;
                        _objectiveManager.DisplayObjective(objectives[2]);
                        _currentState = LevelState3.AccessToIncenerator;
                    }
                }
                else if (_inceneratorSwitchDialogue.finished)
                {
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                break;

            case LevelState3.AccessToIncenerator:
                if (_inceneratorDialogue.finished)
                {
                    _objectiveManager.DisplayObjective(objectives[3]);
                    _final.SetActive(true);
                    _currentState = LevelState3.ExitFromIncinerator;
                }
                break;

            case LevelState3.ExitFromIncinerator:
                if (_finalDialogue.finished)
                {
                    enabled = false;
                }
                break;
        }
    }

    public enum LevelState3
    {
        Start,
        Introduction,
        ApproachToGarbageShoot,
        DeactivateTheIncinerator,
        AccessToIncenerator,
        ExitFromIncinerator
    }
}
