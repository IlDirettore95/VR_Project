using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathPopUp : MonoBehaviour
{
    [SerializeField] private Text _scrapped;

    [SerializeField] private Button _continue;

    [SerializeField] private Button _exit;

    [SerializeField] private Image trashcan;

    [SerializeField] private Image crosshair;

    [SerializeField] private LevelSystem levelSys;

    // Start is called before the first frame update
    public void open()
    {
        gameObject.SetActive(true);
        death();
    }

    public void close()
    {
        gameObject.SetActive(false);
    }
    
    public void death()
    {
        GameEvent.isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        crosshair.gameObject.SetActive(false);
        Time.timeScale = 0f;
    }
    
    public void returnToMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    public void continueGame()
    {
        levelSys.Continue();
    }
}
