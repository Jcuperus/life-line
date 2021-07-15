using System;
using System.Collections;
using Animation;
using Enemies.AttackBehaviour;
using Gameplay.Projectile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemySun : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private float patternShootDelay = 5.5f;

        private FireBehaviour[] attackBehaviours;
        private MultiAttackAnimationBehaviour attackAnimationBehaviour;

        public static event Action OnSunDefeated;
        /**********************************************/
    
        /******************* INIT *********************/
        private void Start()
        {
            if (animationController.AttackAnimation is MultiAttackAnimationBehaviour behaviour)
            {
                attackAnimationBehaviour = behaviour;
            }
            else
            {
                throw new InvalidCastException("Cast Invalid. Expected: MultiAttackAnimationBehaviour");
            }

            attackBehaviours = new FireBehaviour[]
            {
                new BulletStream(ProjectileFactory.ProjectileTypes.EnemyRicochet, transform, player.transform),
                new BulletArc(ProjectileFactory.ProjectileTypes.Enemy, transform, player.transform),
                new BulletCircle(ProjectileFactory.ProjectileTypes.EnemyRicochet, transform)
            };
            
            StartCoroutine(AttackCoroutine());
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        private IEnumerator AttackCoroutine()
        {
            int behaviourIndex = 0;

            while (gameObject.activeSelf)
            {
                FireBehaviour currentBehaviour = attackBehaviours[behaviourIndex];
                StartCoroutine(currentBehaviour.Execute());
            
                attackAnimationBehaviour.Play(behaviourIndex);
            
                behaviourIndex = Random.Range(0, attackBehaviours.Length);
                yield return new WaitForSeconds(patternShootDelay);
            }
        }

        protected override void DestroyEnemy()
        {
            OnSunDefeated?.Invoke();
            base.DestroyEnemy();
        }
        /**********************************************/
    }
}
