using UnityEngine;

namespace Animation
{
    public abstract class AttackAnimationBehaviour : MonoBehaviour
    {
        protected ActorAnimationController animationController;
        
        private void Awake()
        {
            animationController = GetComponent<ActorAnimationController>();
        }
        
        public abstract void Play(float timeScale = 1f);
    }
}