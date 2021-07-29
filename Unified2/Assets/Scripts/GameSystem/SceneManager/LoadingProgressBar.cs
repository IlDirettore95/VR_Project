using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    private Image loadingBar;

    private void Awake()
    {
        loadingBar = transform.GetComponent<Image>();
    }

    private void Update()
    {
        loadingBar.fillAmount = Loader.GetLoadingProgress();
    }
}
