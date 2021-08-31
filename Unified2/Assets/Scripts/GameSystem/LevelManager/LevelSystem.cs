using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{
    public enum Scene
    {
        NewGame,
        Level1,
        Level2,
        Level3,
        EndGame
    }

    public Scene _currentLevel; //This is the current level zone reached by the player. When the game is played for the first time is value is NewGame

    [SerializeField] GameObject _saveLogo;

    // Start is called before the first frame update
    private void Awake()
    {
        SaveData data = SaveSystem.Load("save");

        _currentLevel = (Scene)System.Enum.Parse(typeof(Scene), data._currentScene);
    }

    public void NewGame()
    {
        _currentLevel = Scene.Level1;

        Save();

        Loader.Load(_currentLevel.ToString());
    }

    public void Continue()
    {
        Loader.Load(_currentLevel.ToString());
    }

    public void NextLevel()
    {
        switch (_currentLevel)
        {
            case Scene.Level1:
                {
                    SceneManager.UnloadSceneAsync(_currentLevel.ToString());

                    _currentLevel = Scene.Level2;

                    Save();

                    break;
                }

            case Scene.Level2:
                {
                    SceneManager.UnloadSceneAsync(_currentLevel.ToString());

                    _currentLevel = Scene.Level3;

                    Save();

                    break;
                }
        }
    }

    public void EndLevel()
    {
        switch (_currentLevel)
        {
            case Scene.Level1:
                {
                    SceneManager.LoadSceneAsync(Scene.Level2.ToString(), LoadSceneMode.Additive);

                    break;
                }

            case Scene.Level2:
                {
                    SceneManager.LoadSceneAsync(Scene.Level3.ToString(), LoadSceneMode.Additive);

                    break;
                }

            case Scene.Level3:
                {
                    SceneManager.LoadScene(Scene.EndGame.ToString());

                    break;
                }
        }
    }

    private void Save()
    {
        _saveLogo.SetActive(true); //Active the save icon to alerts the player that the game is saving and the game must not be quitted until it finishes.

        SaveData oldData = SaveSystem.Load("save");
        int dp = oldData.dp;
        int quality = oldData.quality;

        SaveData newData = new SaveData(_currentLevel.ToString(), dp, quality);

        SaveSystem.Save(newData, "save");

        StartCoroutine(EndSave());
    }

    //Used to hide the save icon after two seconds has passed.
    private IEnumerator EndSave()
    {
        yield return new WaitForSeconds(2f);

        _saveLogo.SetActive(false);
    }
}
