using UnityEngine;

namespace Utility
{
    public class MusicSource : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool playsImmediately;
        
        public delegate void PlayMusicAction(AudioClip clip);
        public static event PlayMusicAction OnPlayMusic;

        private void Start()
        {
            if (playsImmediately) Play();
        }

        public void Play()
        {
            OnPlayMusic?.Invoke(clip);
        }
    }
}