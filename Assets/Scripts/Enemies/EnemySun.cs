using System;
using System.Collections;
using Enemies.AttackBehaviour;
using Gameplay.Projectile;
using Spine;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemySun : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private AnimationReferenceAsset idleAnimation;
        [SerializeField] private AnimationReferenceAsset attackAAnimation;
        [SerializeField] private AnimationReferenceAsset attackBAnimation;
        [SerializeField] private AnimationReferenceAsset attackCAnimation;
        [SerializeField] private AnimationReferenceAsset hurtAnimation;
        [SerializeField] private AnimationReferenceAsset deathAnimation;
    
        [Header("Projectile")]
        [SerializeField] private float patternShootDelay = 5.5f;

        private FireBehaviour[] attackBehaviours;
        private AnimationReferenceAsset[] attackAnimations;

        public static event Action OnSunDefeated;
        /**********************************************/
    
        /******************* INIT *********************/
        protected override void Awake()
        {
            base.Awake();

            attackBehaviours = new FireBehaviour[]
            {
                new BulletStream(ProjectileFactory.ProjectileTypes.EnemyRicochet, transform, player.transform),
                new BulletArc(ProjectileFactory.ProjectileTypes.Enemy, transform, player.transform),
                new BulletCircle(ProjectileFactory.ProjectileTypes.EnemyRicochet, transform)
            };

            attackAnimations = new[]
            {
                attackAAnimation, attackBAnimation, attackCAnimation
            };
        }
    
        private void Start()
        {
            currentState = AnimationState.Idle;
            SetAnimation(idleAnimation, true, 1f);
            StartCoroutine(AttackCoroutine());
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        protected override void OnAnimationComplete(TrackEntry trackEntry)
        {
            switch (currentState)
            {
                case AnimationState.Attacking:
                case AnimationState.Hurt:
                    currentState = AnimationState.Idle;
                    SetAnimation(idleAnimation, true, 1f);
                    break;
                case AnimationState.Death:
                    DestroyEnemy();
                    OnSunDefeated?.Invoke();
                    break;
            }
        }
    
        private IEnumerator AttackCoroutine()
        {
            int behaviourIndex = 0;

            while (gameObject.activeSelf)
            {
                FireBehaviour currentBehaviour = attackBehaviours[behaviourIndex];
                StartCoroutine(currentBehaviour.Execute());
            
                SetAnimation(attackAnimations[behaviourIndex], false, 1f);
                currentState = AnimationState.Attacking;
            
                behaviourIndex = Random.Range(0, attackBehaviours.Length);
                yield return new WaitForSeconds(patternShootDelay);
            }
        }
    
        public override void OnProjectileHit(Projectile projectile)
        {
            base.OnProjectileHit(projectile);

            if (currentState == AnimationState.Hurt)
            {
                SetAnimation(hurtAnimation, false, 1f);
            }
            if (currentState == AnimationState.Death)
            {
                SetAnimation(deathAnimation, false, 1f);
                canInterruptAnimation = false;
            }
        }
        /**********************************************/
    }
}
