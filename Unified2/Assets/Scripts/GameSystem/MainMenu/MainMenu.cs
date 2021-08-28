using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject loadingInterface;
    [SerializeField] private Image loadingProgressBar;
    [SerializeField] private Button _continueButton;

    [SerializeField] private Dropdown drop;
    [SerializeField] private Text dropText;

    [SerializeField] private LevelSystem levelSys;

    [SerializeField] private AudioClip _mainMenuMusic;
    private MusicManager _musicManager;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SaveData data = SaveSystem.Load("save");
        if (data != null)
        {
            int dpValue = data.dp;
            int quality = data.quality;

            QualitySettings.SetQualityLevel(quality, true);
            drop.value = dpValue;
        }
        else
        {
            int dpValue = 2;
            int quality = 5;

            QualitySettings.SetQualityLevel(quality, true);
            //drop.value = dpValue;

            SaveData newData = new SaveData("NewGame", dpValue, quality);
            SaveSystem.Save(newData, "save");
        }

        //Play MenuMusic
        _musicManager = GameObject.FindObjectOfType<MusicManager>();
        _musicManager.PlayMusicFade(_mainMenuMusic, 0.3f, 0f);
        ShowMenu();
    }

    public void NewGame()
    {
        levelSys.NewGame();
    }

    public void ContinueGame()
    {
        levelSys.Continue();
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Break();
    }

    public void HideMenu()
    {
        menu.SetActive(false);
    }

    private void ShowMenu()
    {
        menu.SetActive(true);

        if (levelSys._currentLevel.ToString().Equals("NewGame"))
        {
            _continueButton.interactable = false;
        }
        else
        {
            _continueButton.interactable = true;
        }
    }

    public void ShowLoadingScreen()
    {
        loadingInterface.SetActive(true);
    }

    public void ShowSettings()
    {
        settings.SetActive(true);
    }

    public void HideSettings()
    {
        settings.SetActive(false);
    }

    public void OpenSettings()
    {
        HideMenu();
        ShowSettings();
    }

    public void CloseSettings()
    {
        HideSettings();
        ShowMenu();
    }

    public void ChangeGraphics()
    {
        int quality = 5;    //default : ultra

        if (drop.value == 0)
        {
            quality = 2;  //medium
        }
        else if (drop.value == 1)
        {
            quality = 3;    //high
        }
        else
        {
            quality = 5;    //ultra
        }

        SaveData data = SaveSystem.Load("save");
        QualitySettings.SetQualityLevel(quality, true);

        SaveData newData = new SaveData(data._currentScene, drop.value, quality);
        SaveSystem.Save(newData, "save");
    }
}
