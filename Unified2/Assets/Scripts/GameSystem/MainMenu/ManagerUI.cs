using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    [SerializeField] private Button newGame;
    [SerializeField] private Button continueGame;
    [SerializeField] private Button options;
    [SerializeField] private Button exitGame;
    [SerializeField] private Button back;
    
    [SerializeField] private Image backGround;

    [SerializeField] private LevelSystem levelSys;

    [SerializeField] private Dropdown drop;
    [SerializeField] private Text dropText;

    public void OpenSettings()
    {
        backGround.gameObject.SetActive(true);
        back.gameObject.SetActive(true);
        drop.gameObject.SetActive(true);
        dropText.enabled = true;
        
        newGame.gameObject.SetActive(false);
        continueGame.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        exitGame.gameObject.SetActive(false);
        
    }

    public void CloseSettings()
    {
        backGround.gameObject.SetActive(false);
        back.gameObject.SetActive(false);
        drop.gameObject.SetActive(false);
        dropText.enabled = false;

        newGame.gameObject.SetActive(true);
        continueGame.gameObject.SetActive(true);
        options.gameObject.SetActive(true);
        exitGame.gameObject.SetActive(true);
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
}
