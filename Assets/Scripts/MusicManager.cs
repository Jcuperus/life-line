using UnityEngine;
using Utility;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoSingleton<MusicManager>
{
    private AudioSource audioSource;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        MusicSource.OnPlayMusic += Play;
    }

    public void Play(AudioClip clip)
    {
        if (clip == audioSource.clip) return;

        audioSource.clip = clip;
        audioSource.Play();;
    }

    public void Stop()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }
}