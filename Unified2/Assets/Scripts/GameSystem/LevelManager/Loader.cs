using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonoBehaviour : MonoBehaviour { }  //dummy class

    private static Action onLoaderCallback;
    private static List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private static float totalProgress;

    public static void Load(string scene)
    {
        onLoaderCallback = () =>
        {
            GameObject loadingGameObject = new GameObject("Loading_Game_Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };

        SceneManager.LoadScene("Loading");
    }

    public static void LoaderCallback()
    {
        if(onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }

    public static float GetLoadingProgress() => totalProgress;

    private static IEnumerator LoadSceneAsync(string scene)
    {
        yield return null;

        scenesToLoad.Clear();

        //Load the scenes asynchronously in additive mode
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Gameplay"));
        scenesToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));

        totalProgress = 0;

        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                //Adding the scene progress to the total progress
                totalProgress += scenesToLoad[i].progress;
                //the fillAmount needs a value between 0 and 1, so we devide the progress by the number of scenes to load
                totalProgress = totalProgress / scenesToLoad.Count;
                yield return null;
            }
        }    
    }
}
