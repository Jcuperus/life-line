using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Animation
{
    [RequireComponent(typeof(SkeletonAnimation), typeof(AttackAnimationBehaviour))]
    public class ActorAnimationController : MonoBehaviour
    {
        [SerializeField] private AnimationReferenceAsset idleAnimation; 
        [SerializeField] private AnimationReferenceAsset hurtAnimation;
        [SerializeField] private AnimationReferenceAsset deathAnimation;
        
        private SkeletonAnimation animator;
        public AttackAnimationBehaviour AttackAnimation { get; set; }

        public enum AnimationState
        {
            Idle,
            Hurt,
            Attacking,
            Death
        }

        [NonSerialized] public AnimationState currentState;
        private bool canInterruptAnimation = true;
        
        public event Action OnDeathAnimationFinished;

        private void Awake()
        {
            animator = GetComponent<SkeletonAnimation>();
            AttackAnimation = GetComponent<AttackAnimationBehaviour>();
        }

        private void Start()
        {
            currentState = AnimationState.Idle;
            SetAnimation(idleAnimation, true, 1f);
        }

        private void OnAnimationComplete(TrackEntry trackEntry)
        {
            switch (currentState)
            {
                case AnimationState.Death:
                    OnDeathAnimationFinished?.Invoke();
                    break;
                default:
                    currentState = AnimationState.Idle;
                    SetAnimation(idleAnimation, true, 1f);
                    break;
            }
        }
        
        public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
        {
            if (!canInterruptAnimation || animation == null) return;
        
            TrackEntry trackEntry = animator.state.SetAnimation(0, animation, loop);
            trackEntry.TimeScale = timeScale;
            trackEntry.Complete += OnAnimationComplete;
        }

        public void PlayHurtAnimation(float timeScale = 1f)
        {
            currentState = AnimationState.Hurt;
            SetAnimation(hurtAnimation, false, timeScale);
        }

        public void PlayDeathAnimation(float timeScale = 1f)
        {
            currentState = AnimationState.Death;
            SetAnimation(deathAnimation, false, timeScale);
            canInterruptAnimation = false;
        }
    }
}