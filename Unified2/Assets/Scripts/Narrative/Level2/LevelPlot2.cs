using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class handles level 1's objectives (plot)
 * Every state rappresent an objective:
 *      Start: setting
 *      Introduction: first dialogue
 *      TutorialFight:
 *      PostFight,
 *      Fight,
 *      ContinueToIndustrial
 */
public class LevelPlot2 : MonoBehaviour
{
    private GameObject _player;

    private LevelState2 _currentState; //Current state of the plot state machine

    private DialogueTrigger[] dialogues; //List of dialogues plot will trigger

    private MovementSystem _movementSystem;

    [SerializeField] Enemy _drone;
    [SerializeField] Enemy[] _enemies;

    [SerializeField] private GameObject _fightTutorial;
    private DialogueTrigger _fightTutorialDialogue;

    [SerializeField] private GameObject _fight;
    private DialogueTrigger _fightDialogue;

    [SerializeField] private GameObject _industrialAccess;
    private DialogueTrigger _industrialAccessDialogue;

    [SerializeField] private GameObject _terminalStation;
    private DialogueTrigger _terminalStationDialogue;
    private BoxCollider _terminalStationCollider;

    [SerializeField] private GameObject _firstDoor;
    private UnlockableDoorAnimation _firstDoorAnimation;

    [SerializeField] private GameObject _secondDoor;
    private UnlockableDoorAnimation _secondDoorAnimation;

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
        dialogues = GetComponents<DialogueTrigger>();

        _fightTutorial.SetActive(true);
        _fight.SetActive(false);
        _industrialAccess.SetActive(false);

        _fightTutorialDialogue = _fightTutorial.GetComponent<DialogueTrigger>();
        _fightDialogue = _fight.GetComponent<DialogueTrigger>();
        _industrialAccessDialogue = _industrialAccess.GetComponent<DialogueTrigger>();
        _terminalStationDialogue = _terminalStation.GetComponent<DialogueTrigger>();
        _terminalStationCollider = _terminalStation.GetComponent<BoxCollider>();
        _terminalStationCollider.enabled = false;

        _firstDoorAnimation = _firstDoor.GetComponentInChildren<UnlockableDoorAnimation>();
        _firstDoorAnimation.LockDoor();
        _secondDoorAnimation = _secondDoor.GetComponentInChildren<UnlockableDoorAnimation>();
        _secondDoorAnimation.LockDoor();
        _enterDoorAnimation = _enterDoor.GetComponentInChildren<UnlockableDoorAnimation>();
        _enterDoorAnimation.LockDoor();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case LevelState2.Start:
                if (_movementSystem.IsLanded())
                {
                    dialogues[0].TriggerDialogue();
                    _currentState = LevelState2.Introduction;
                }
                break;

            case LevelState2.Introduction:
                if (dialogues[0].finished)
                {
                    _fightTutorial.SetActive(true);
                    _objectiveManager.DisplayObjective(objectives[0]);
                    _currentState = LevelState2.PreTutorialFight;
                }
                break;

            case LevelState2.PreTutorialFight:
                if(_fightTutorialDialogue.finished)
                {
                    _objectiveManager.DisplayObjective(objectives[1]);
                    _currentState = LevelState2.TutorialFight;
                }
                break;

            case LevelState2.TutorialFight:
                if (objectiveDone)
                {
                    if (Time.time > nextTimeObjective)
                    {
                        objectiveDone = false;
                        dialogues[1].TriggerDialogue();
                        _terminalStationCollider.enabled = true;
                        _firstDoorAnimation.UnLockDoor();
                        _currentState = LevelState2.PostTutorialFight;
                    }
                }
                else if (!_drone.isAlive)
                {
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                break;

            case LevelState2.PostTutorialFight:             
                if (dialogues[1].finished)
                {
                    _objectiveManager.DisplayObjective(objectives[2]);
                    _fight.SetActive(true);
                    _currentState = LevelState2.PreFight;
                }
                break;

            case LevelState2.PreFight:
                if (_fightDialogue.finished)
                {
                    _objectiveManager.DisplayObjective(objectives[3]);
                    _firstDoorAnimation.LockDoor();
                    _currentState = LevelState2.Fight;
                }
                break;

            case LevelState2.Fight:
                if (objectiveDone)
                {
                    if (Time.time > nextTimeObjective)
                    {
                        objectiveDone = false;
                        dialogues[2].TriggerDialogue();
                        _secondDoorAnimation.UnLockDoor();
                        _firstDoorAnimation.UnLockDoor();
                        _currentState = LevelState2.PostFight;
                    }
                }
                else if (EnemiesEliminated())
                {
                    objectiveDone = true;
                    nextTimeObjective = Time.time + objectiveDelay;
                }
                break;

            case LevelState2.PostFight:
                if(dialogues[2].finished)
                {
                    _objectiveManager.DisplayObjective(objectives[4]);
                    _industrialAccess.SetActive(true);
                    _currentState = LevelState2.ContinueToIndustrial;
                }
                break;

            case LevelState2.ContinueToIndustrial:
                if(_industrialAccessDialogue.finished)
                {
                    enabled = false;
                }
                break;
        }
    }

    private bool EnemiesEliminated()
    {
        for(int i = 0; i < _enemies.Length; i++)
        {
            if (_enemies[i].isAlive) return false;
        }
        return true;
    }

    public enum LevelState2
    {
        Start,
        Introduction,
        PreTutorialFight,
        TutorialFight,
        PostTutorialFight,
        PreFight,
        Fight,
        PostFight,
        ContinueToIndustrial
    }
}
