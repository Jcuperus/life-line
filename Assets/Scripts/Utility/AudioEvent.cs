using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// Class for creating <see cref="AudioEvent"/> Assets, which allow <see cref="MonoBehaviour"/>s to play <see cref="AudioClip"/>s in varrying pitch and volume.
/// </summary>
[CreateAssetMenu(fileName = "Unnamed AudioEvent", menuName = "Custom Assets/Utility/AudioEvent")]
public class AudioEvent : ScriptableObject
{
    /**************** VARIABLES *******************/
    [SerializeField] private AudioClip[] clips;
    [SerializeField] [Range(0.1f, 10)] private float volumeMin;
    [SerializeField] [Range(0.1f, 10)] private float volumeMax;

    [SerializeField] [Range(0.1f, 10)] private float pitchMin;
    [SerializeField] [Range(0.1f, 10)] private float pitchMax;
    
    public void Play(AudioSource audioSource)
    {
        if (clips.Length == 0) return;
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.Play();
    }
    /**********************************************/
}
