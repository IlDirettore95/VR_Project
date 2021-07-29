using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{
    public enum Scene
    {
        Level1,
        Level2,
        Loading
    }

    public Scene _currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        SaveData data = SaveSystem.Load("save");

        _currentLevel = (Scene)System.Enum.Parse(typeof(Scene), data._currentScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevel()
    {
        switch (_currentLevel)
        {
            case Scene.Level1:
                {
                    SceneManager.UnloadSceneAsync(_currentLevel.ToString());

                    _currentLevel = Scene.Level2;
                    SaveData oldData = SaveSystem.Load("save");
                    int dp = oldData.dp;
                    int quality = oldData.quality;
                    SaveData newData = new SaveData(_currentLevel.ToString(), dp, quality);
                    SaveSystem.Save(newData, "save");

                    break;
                }
        }
    }

    public void EndGame()
    {
        SceneManager.LoadScene("EndDemo");
    }

    public void NewGame()
    {
        _currentLevel = Scene.Level1;
        SaveData oldData = SaveSystem.Load("save");
        int dp = oldData.dp;
        int quality = oldData.quality;
        SaveData newData = new SaveData(_currentLevel.ToString(), dp, quality);
        SaveSystem.Save(newData, "save");
        Loader.Load(_currentLevel.ToString());
    }

    public void Continue()
    {
        SaveData data = SaveSystem.Load("save");
        //_currentLevel = (Scene)System.Enum.Parse(typeof(Scene),data._currentScene);
        Loader.Load(_currentLevel.ToString());
    }
}
