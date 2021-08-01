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
                    _objs
                }
                break;
            case LevelState2.TutorialFight:
                break;
            case LevelState2.PostFight:
                break;
            case LevelState2.Fight:
                break;
            case LevelState2.ContinueToIndustrial:
                break;
        }
    }

    public enum LevelState2
    {
        Start,
        Introduction,
        TutorialFight,
        PostFight,
        Fight,
        ContinueToIndustrial
    }
}
