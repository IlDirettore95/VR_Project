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


        /*
        HideMenu();
         
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Gameplay"));

        scenesToLoad.Add(SceneManager.LoadSceneAsync("Level1", LoadSceneMode.Additive));
        StartCoroutine(LoadingScreen());
        */
    }

    public void ContinueGame()
    {
        levelSys.Continue();

        /*
        HideMenu();
        ShowLoadingScreen();

        scenesToLoad.Add(SceneManager.LoadSceneAsync("Gameplay"));
        */
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

    
    IEnumerator LoadingScreen()
    {
        ShowLoadingScreen();

        float totalProgress = 0;

        loadingProgressBar.fillAmount = totalProgress;

        //Iterate through all the scenes to load
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                //Adding the scene progress to the total progress
                totalProgress += scenesToLoad[i].progress;
                //the fillAmount needs a value between 0 and 1, so we devide the progress by the number of scenes to load
                loadingProgressBar.fillAmount = totalProgress / scenesToLoad.Count;
                yield return null;
            }
        }
    }
    
}
