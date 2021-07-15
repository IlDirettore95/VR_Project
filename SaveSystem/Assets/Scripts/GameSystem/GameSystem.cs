using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public enum Scene
    {
        Level1,
        Level2,
        Loading
    }

    private Scene _currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        
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
                    _currentLevel = Scene.Level2;
                    SaveSystem.Save(_currentLevel.ToString(), "save");
                    Loader.Load(_currentLevel.ToString());

                    break;
                }
        }
    }

    public void NewGame()
    {
        _currentLevel = Scene.Level1;
        SaveSystem.Save(_currentLevel.ToString(), "save");
        Loader.Load(_currentLevel.ToString());
    }

    public void Continue()
    {
        _currentLevel = (Scene)System.Enum.Parse(typeof(Scene),SaveSystem.Load("save"));
        Loader.Load(_currentLevel.ToString());
    }
}
