using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class handles game music
 */
public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioSource _auxAudioSource; //Used in transition

    public static MusicManager instance;

    private void Awake()
    {
        //Singleton Pattern to avoid multiple istances
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        _audioSource = gameObject.AddComponent<AudioSource>();
        _auxAudioSource = gameObject.AddComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        _audioSource.loop = true;
        _audioSource.spatialBlend = 0f;
        _audioSource.playOnAwake = false;

        
        _auxAudioSource.loop = true;
        _auxAudioSource.spatialBlend = 0f;
        _auxAudioSource.playOnAwake = false;

    }

    public void PlayMusic(AudioClip audioClip, float delay)
    {
        if(!audioClip.Equals(_audioSource.clip))
        {
            _audioSource.clip = audioClip;
            _audioSource.PlayDelayed(delay);
        }
        
    }

    public void PlayMusicFade(AudioClip audioClip, float fadeInDuration, float delay)
    {
        if (!audioClip.Equals(_audioSource.clip))
        {
            _audioSource.clip = audioClip;
            _audioSource.volume = 0f;
            _audioSource.PlayDelayed(delay);
            StartCoroutine(FadeIn(_audioSource, fadeInDuration));
        }
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }

    public void StopMusic(float fadeInDuration)
    {
        StartCoroutine(FadeOut(_audioSource, fadeInDuration));
    }

    public void PlayMusicTransition(AudioClip audioClip, float fadeInDuration, float fadeOutDuration)
    {
        if (!audioClip.Equals(_audioSource.clip))
        {
            StartCoroutine(Transition(audioClip, fadeInDuration, fadeOutDuration));
        }
    }

    public void PlayMusicOverlapTransition(AudioClip audioClip, float fadeInDuration, float fadeOutDuration, float delayIn)
    {
        if (!audioClip.Equals(_audioSource.clip))
        {
            StartCoroutine(OverlapTransition(audioClip, fadeInDuration, fadeOutDuration, delayIn));
        }
    }

    private IEnumerator FadeIn(AudioSource source, float duration)
    {
        float incrementPerSecond = 1 / duration;
        while(true)
        {
            source.volume += incrementPerSecond * Time.deltaTime;
            if (source.volume == 1) break;
            else yield return null;
        }
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float decrementPerSecond = 1 / duration;
        while (true)
        {
            source.volume -= decrementPerSecond * Time.deltaTime;
            if (source.volume == 0)
            {
                source.clip = null;
                break;
            }
            else yield return null;
        }
    }

    private IEnumerator Transition(AudioClip nextMusic, float fadeInDuration, float fadeOutDuration)
    {
        if (!nextMusic.Equals(_audioSource.clip))
        {
            float incrementPerSecond = 1 / fadeInDuration;
            float decrementPerSecond = 1 / fadeOutDuration;

            StartCoroutine(FadeOut(_audioSource, fadeOutDuration));
            while (true)
            {
                if (_audioSource.volume == 0)
                {
                    break;
                }
                else yield return null;
            }

            _audioSource.clip = nextMusic;
            _audioSource.Play();

            StartCoroutine(FadeIn(_audioSource, fadeInDuration));
        }
    }

    private IEnumerator OverlapTransition(AudioClip nextMusic, float fadeInDuration, float fadeOutDuration, float delayIn)
    {
        if (!nextMusic.Equals(_audioSource.clip))
        {
            _auxAudioSource.volume = 0f;
            _auxAudioSource.clip = nextMusic;
            _auxAudioSource.PlayDelayed(delayIn);

            StartCoroutine(FadeOut(_audioSource, fadeOutDuration));

            yield return new WaitForSeconds(delayIn);

            StartCoroutine(FadeIn(_auxAudioSource, fadeInDuration));

            while (true)
            {
                if (_auxAudioSource.volume == 1)
                {
                    break;
                }
                else yield return null;
            }

            AudioSource tmp = new AudioSource();
            tmp = _audioSource;
            _audioSource = _auxAudioSource;
            _auxAudioSource = tmp;
        }
    }
}
