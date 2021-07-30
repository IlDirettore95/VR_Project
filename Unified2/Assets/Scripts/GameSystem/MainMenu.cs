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

    [SerializeField] private Dropdown drop;
    [SerializeField] private Text dropText;

    [SerializeField] private LevelSystem levelSys;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private void Start()
    {
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

            SaveData newData = new SaveData("Level1", dpValue, quality);
            SaveSystem.Save(newData, "save");
        }
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
        menu.SetActive(true);
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
