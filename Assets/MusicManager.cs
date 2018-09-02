using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance;

    public AudioClip backgroundMusic;

    public AudioClip wind;

    public float fadeTime = 1;

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayMusic()
    {
        StartCoroutine(FadeMusic(backgroundMusic));
    }

    public void PlayWind()
    {
        StartCoroutine(FadeMusic(wind));
    }

    private IEnumerator FadeMusic(AudioClip nextClip)
    {
        if (audioSource.isPlaying)
        {
            while (audioSource.volume > 0.0f)
            {
                audioSource.volume -= Time.deltaTime / fadeTime;
                yield return null;
            }
        }
        
        audioSource.clip = nextClip;
        audioSource.Play();

        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}
