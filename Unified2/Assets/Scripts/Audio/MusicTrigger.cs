using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] AudioClip _music;
    private MusicManager _musicManager;

    // Start is called before the first frame update
    void Start()
    {
        _musicManager = GameObject.FindObjectOfType<MusicManager>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            _musicManager.PlayMusicOverlapTransition(_music, 4f, 1f, 0f);
            gameObject.SetActive(false);
        }
    }
}
