using System;
using System.Collections;
using Animation;
using Gameplay.AttackBehaviour;
using Gameplay.Projectile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemySun : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private float patternShootDelay = 0.5f;

        private struct AttackConfiguration
        {
            public FireBehaviour attackBehaviour;
            public ProjectileFactory.ProjectileTypes projectileType;
        }

        private AttackConfiguration[] attackConfigurations;
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

            Transform origin = transform;
            Transform playerTransform = player.transform;
            
            attackConfigurations = new[]
            {
                new AttackConfiguration
                {
                    attackBehaviour = new BulletStream(origin, playerTransform),
                    projectileType = ProjectileFactory.ProjectileTypes.EnemyRicochet
                },
                new AttackConfiguration
                {
                    attackBehaviour = new BulletArc(origin, playerTransform),
                    projectileType = ProjectileFactory.ProjectileTypes.Enemy
                },
                new AttackConfiguration
                {
                    attackBehaviour = new BulletCircle(origin),
                    projectileType = ProjectileFactory.ProjectileTypes.EnemyRicochet
                }
            };
            
            StartCoroutine(AttackCoroutine());
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        private IEnumerator AttackCoroutine()
        {
            int configurationIndex = 0;

            while (isAlive)
            {
                yield return new WaitForSeconds(patternShootDelay);

                attackAnimationBehaviour.Play(configurationIndex);
                AttackConfiguration configuration = attackConfigurations[configurationIndex];

                yield return configuration.attackBehaviour.Execute(configuration.projectileType);

                configurationIndex = Random.Range(0, attackConfigurations.Length);
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
