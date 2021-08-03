using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoToMenu : MonoBehaviour
{

    [SerializeField] private VideoPlayer _videoPlayer;

   

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(checkVideoPlayer());
       
        
    }

    public IEnumerator checkVideoPlayer()
    {
        
        
        yield return new WaitForSeconds((float)_videoPlayer.length);
        Debug.Log("finito");

        SceneManager.LoadScene("MainMenu");

    }
}
