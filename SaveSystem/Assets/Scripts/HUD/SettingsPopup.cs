using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    public Image crosshair;

    public Button riprendi;

    public Button Esci;

    public Button Opzioni;

    public Button indietro;

    public Text pausa;

    public Text impostazioni;

    public Text graphics;

    public Dropdown dp;

    public GameObject dialogueBox;

    public GameObject objectiveBox;

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

        pausa.enabled = true;
        riprendi.gameObject.SetActive(true);
        Opzioni.gameObject.SetActive(true);
        Esci.gameObject.SetActive(true);

        indietro.gameObject.SetActive(false);
        impostazioni.enabled = false;
        graphics.enabled = false;
        dp.gameObject.SetActive(false);

        dialogueBox.SetActive(false);
    }

    public void UnPauseGame()
    {
        GameEvent.isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crosshair.gameObject.SetActive(true);
        Time.timeScale = 1f;

        dialogueBox.SetActive(true);

    }

    public void openSettings()
    {
        riprendi.gameObject.SetActive(false);
        Opzioni.gameObject.SetActive(false);
        Esci.gameObject.SetActive(false);
        pausa.enabled = false;

        impostazioni.enabled = true;
        indietro.gameObject.SetActive(true);
        graphics.enabled = true;
        dp.gameObject.SetActive(true);
    }

    public void closeSettings()
    {
        pausa.enabled = true;
        riprendi.gameObject.SetActive(true);
        Opzioni.gameObject.SetActive(true);
        Esci.gameObject.SetActive(true);


        impostazioni.enabled = false;
        indietro.gameObject.SetActive(false);
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

        Debug.Log(dp.value);

        int quality = 5;    //default : ultra

        if (dp.value == 0)
        {
            Debug.Log("Medium");
            quality = 2;  //medium
        }
        else if (dp.value == 1)
        {
            Debug.Log("High");
            quality = 3;    //high
        }
        else
        {
            Debug.Log("Ultra");
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

