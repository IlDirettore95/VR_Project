using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    public Image crosshair;
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
    }
    
    public void UnPauseGame()
    {
        GameEvent.isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crosshair.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }
}

