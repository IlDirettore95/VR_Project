using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private bool wasOnDialoge = false;

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

        AudioListener.pause = true;
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
            wasOnDialoge = true;
            dialogueBox.SetActive(false);
        }
    }

    public void UnPauseGame()
    {
        GameEvent.isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crosshair.gameObject.SetActive(true);

        AudioListener.pause = false;
        Time.timeScale = 1f;

        if (wasOnDialoge)
        {
            dialogueBox.SetActive(true);
            wasOnDialoge = false;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    public void ChangeGraphics()
    {
        int quality = 5;    //default : ultra

        if (dp.value == 0)
        {
            quality = 2;  //medium
        }
        else if (dp.value == 1)
        {
            quality = 3;    //high
        }
        else
        {
            quality = 5;    //ultra
        }

        SaveData data = SaveSystem.Load("save");
        QualitySettings.SetQualityLevel(quality, true);

        SaveData newData = new SaveData(data._currentScene, dp.value, quality);
        SaveSystem.Save(newData, "save");
    }

    private void Start()
    {
        SaveData data = SaveSystem.Load("save");
        int dpValue = data.dp;
        dp.value = dpValue;
    }
}

