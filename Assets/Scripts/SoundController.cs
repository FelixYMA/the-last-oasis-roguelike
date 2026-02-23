using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource audioSource;
    public AudioClip[] soundTracks;

    private int currentTrackIndex = 0;
    public float fadeDuration = 1.5f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        audioSource = GetComponent<AudioSource>(); 
    }

    void Start()
    {
        PlaySound(currentTrackIndex); 
    }

    public void PlaySound(int trackIndex)
    {
        if (trackIndex >= 0 && trackIndex < soundTracks.Length)
        {
            StartCoroutine(FadeAndPlay(trackIndex)); 
            currentTrackIndex = trackIndex;
        }
    }

    public void NextSound()
    {
        currentTrackIndex = (currentTrackIndex + 1) % soundTracks.Length;
        PlaySound(currentTrackIndex);
    }

    IEnumerator FadeAndPlay(int trackIndex)
    {
        float startVolume = audioSource.volume;


        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }


        audioSource.clip = soundTracks[trackIndex];
        audioSource.Play();


        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextSound();
        }

        if (Time.timeSinceLevelLoad > 30f && currentTrackIndex != 1)
        {
            PlaySound(1);
        }
        if (Time.timeSinceLevelLoad > 60f && currentTrackIndex != 2)
        {
            PlaySound(2);
        }
    }
}
