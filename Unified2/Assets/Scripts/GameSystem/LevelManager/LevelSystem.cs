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

    public Scene _currentLevel;

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
        SaveData data = SaveSystem.Load("save");

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
        _saveLogo.SetActive(true);

        SaveData oldData = SaveSystem.Load("save");
        int dp = oldData.dp;
        int quality = oldData.quality;

        SaveData newData = new SaveData(_currentLevel.ToString(), dp, quality);

        SaveSystem.Save(newData, "save");

        StartCoroutine(EndSave());
    }

    private IEnumerator EndSave()
    {
        yield return new WaitForSeconds(2f);

        _saveLogo.SetActive(false);
    }
}
