using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Animation
{
    [RequireComponent(typeof(SkeletonAnimation), typeof(AttackAnimationBehaviour))]
    public class ActorAnimationController : MonoBehaviour
    {
        [SerializeField] private AnimationReferenceAsset idleAnimation, hurtAnimation, deathAnimation, spawnAnimation;
        
        private SkeletonAnimation animator;
        public AttackAnimationBehaviour AttackAnimation { get; set; }

        public enum AnimationState
        {
            Idle,
            Hurt,
            Attacking,
            Death,
            Spawn
        }
        
        public AnimationState CurrentState
        {
            get => currentState;
            set
            {
                if (canInterruptAnimation) currentState = value;
            }
        }

        private AnimationState currentState;
        
        private bool canInterruptAnimation = true;
        
        public event Action OnDeathAnimationFinished, OnSpawnAnimationFinished;

        private void Awake()
        {
            animator = GetComponent<SkeletonAnimation>();
            AttackAnimation = GetComponent<AttackAnimationBehaviour>();
        }

        private void Start()
        {
            if (HasSpawnAnimation())
            {
                PlaySpawnAnimation();
            }
            else
            {
                CurrentState = AnimationState.Idle;
                SetAnimation(idleAnimation, true, 1f);
            }
        }

        private void OnAnimationComplete(TrackEntry trackEntry)
        {
            switch (CurrentState)
            {
                case AnimationState.Death:
                    OnDeathAnimationFinished?.Invoke();
                    break;
                case AnimationState.Spawn:
                    OnSpawnAnimationFinished?.Invoke();
                    SetAnimation(idleAnimation, true, 1f);
                    break;
                default:
                    CurrentState = AnimationState.Idle;
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
            CurrentState = AnimationState.Hurt;
            SetAnimation(hurtAnimation, false, timeScale);
        }

        public void PlayDeathAnimation(float timeScale = 1f)
        {
            CurrentState = AnimationState.Death;
            SetAnimation(deathAnimation, false, timeScale);
            canInterruptAnimation = false;
        }

        public void PlaySpawnAnimation(float timeScale = 1f)
        {
            CurrentState = AnimationState.Spawn;
            SetAnimation(spawnAnimation, false, timeScale);
        }

        public bool HasSpawnAnimation()
        {
            return spawnAnimation != null;
        }
    }
}