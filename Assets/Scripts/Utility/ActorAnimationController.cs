using System;
using System.Runtime.CompilerServices;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Utility
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class ActorAnimationController : MonoBehaviour
    {
        [SerializeField] private AnimationReferenceAsset idleAnimation; 
        [SerializeField] private AnimationReferenceAsset attackAnimation; 
        [SerializeField] private AnimationReferenceAsset deathAnimation;
        
        private SkeletonAnimation animator;
        
        private enum AnimationState
        {
            Idle,
            Hurt,
            Attacking,
            Death
        }

        private AnimationState currentState;
        private bool canInterruptAnimation = true;
        
        public event Action OnDeathAnimationFinished;

        private void Awake()
        {
            animator = GetComponent<SkeletonAnimation>();
            currentState = AnimationState.Idle;
            SetAnimation(idleAnimation, true, 1f);
        }
        
        private void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
        {
            if (!canInterruptAnimation) return;
        
            TrackEntry trackEntry = animator.state.SetAnimation(0, animation, loop);
            trackEntry.TimeScale = timeScale;
            trackEntry.Complete += OnAnimationComplete;
        }

        private void OnAnimationComplete(TrackEntry trackEntry)
        {
            switch (currentState)
            {
                case AnimationState.Attacking:
                    currentState = AnimationState.Idle;
                    SetAnimation(idleAnimation, true, 1f);
                    break;
                case AnimationState.Death:
                    OnDeathAnimationFinished?.Invoke();
                    break;
            }
        }

        public void PlayAttackAnimation()
        {
            currentState = AnimationState.Attacking;
            SetAnimation(attackAnimation, false, 1f);
        }

        public void PlayDeathAnimation()
        {
            currentState = AnimationState.Death;
            SetAnimation(deathAnimation, false, 1f);
            canInterruptAnimation = false;
        }
    }
}