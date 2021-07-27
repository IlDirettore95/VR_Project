using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonoBehaviour : MonoBehaviour { }  //dummy class

    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOp;

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

    public static float GetLoadingProgress()
    {
        if(loadingAsyncOp != null)
        {
            return loadingAsyncOp.progress;
        }
        else
        {
            return 1f;
        }
    }

    private static IEnumerator LoadSceneAsync(string scene)
    {
        yield return null;

        loadingAsyncOp = SceneManager.LoadSceneAsync(scene.ToString());

        while (!loadingAsyncOp.isDone)
        {
            yield return null;
        }
    }
}
