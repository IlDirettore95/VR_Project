using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    public Image crosshair;

    public Button resume;

    public Button exit;

    public Button options;

    public Button back;

    public Text pause;

    public Text settings;

    public Text graphics;

    public Dropdown dp;

    public GameObject dialogueBox;

    public GameObject objectiveBox;

    private bool wasOnDialogue = false;
    
    // Start is called before the first frame update
    public void open()
    {
        gameObject.SetActive(true);
        PauseGame();
    }

    public void close()
    {
        gameObject.SetActive(false);
        UnPauseGame();
    }
    
    public void PauseGame()
    {
        GameEvent.isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        crosshair.gameObject.SetActive(false);
        Time.timeScale = 0f;

        pause.enabled = true;
        resume.gameObject.SetActive(true);
        options.gameObject.SetActive(true);
        exit.gameObject.SetActive(true);

        back.gameObject.SetActive(false);
        settings.enabled = false;
        graphics.enabled = false;
        dp.gameObject.SetActive(false);

        if (dialogueBox.activeSelf)
        {
            wasOnDialogue = true;
            dialogueBox.SetActive(false);
        }
    }
    
    public void UnPauseGame()
    {
        GameEvent.isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crosshair.gameObject.SetActive(true);
        Time.timeScale = 1f;

        if (wasOnDialogue)
        {
            dialogueBox.SetActive(true);
            wasOnDialogue = false;
        }
    }

    public void openSettings()
    {
        resume.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        exit.gameObject.SetActive(false);
        pause.enabled = false;
        
        settings.enabled = true;
        back.gameObject.SetActive(true);
        graphics.enabled = true;
        dp.gameObject.SetActive(true);
    }

    public void closeSettings()
    {
        pause.enabled = true;
        resume.gameObject.SetActive(true);
        options.gameObject.SetActive(true);
        exit.gameObject.SetActive(true);

        
        settings.enabled = false;
        back.gameObject.SetActive(false);
        graphics.enabled = false;
        dp.gameObject.SetActive(false);
    }

    public void quitGame()
    {
        Application.Quit();
        Debug.Break();
    }

    public void ChangeGraphics()
    {
        
        if (dp.value == 0)
        {
            int medium = 2;
            QualitySettings.SetQualityLevel(medium, true);
        }
        else if (dp.value == 1)
        {
            int high = 3;
            QualitySettings.SetQualityLevel(high, true);
        }
        else
        {
            int ultra = 5;
            QualitySettings.SetQualityLevel(ultra, true);
        }
    }

    private void Start()
    {
        dp.value = 2;
    }
}

