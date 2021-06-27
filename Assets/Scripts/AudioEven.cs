#region namespaces
using UnityEngine;
using Random = UnityEngine.Random;
#endregion
[CreateAssetMenu]
public class AudioEven : ScriptableObject
{
    /**********************************************/
    /*                VARIABLES                   */
    /**********************************************/

    [SerializeField] private AudioClip[] clips;
    [SerializeField] [Range(0.1f, 10)] private float volumeMin;
    [SerializeField] [Range(0.1f, 10)] private float volumeMax;

    [SerializeField][Range(0.1f, 10)] private float pitchMin;
    [SerializeField][Range(0.1f, 10)] private float pitchMax;
    public void Play(AudioSource audioSource)
    {
        if (clips.Length == 0) return;
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.Play();
    }

}
/*
public abstract class AudioEventAbstract : ScriptableObject
{
    public abstract void Play(AudioSource audioSource);
}
*/
