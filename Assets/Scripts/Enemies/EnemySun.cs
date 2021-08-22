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
        [SerializeField] private float patternShootDelay = 1.5f;

        [Serializable]
        private struct AttackConfiguration
        {
            public FireBehaviour attackBehaviour;
            public ProjectileFactory.ProjectileTypes projectileType;
        }

        [SerializeField] private AttackConfiguration[] attackConfigurations;
        private MultiAttackAnimationBehaviour attackAnimationBehaviour;

        public static event Action OnSunDefeated;
        /**********************************************/
    
        /******************* INIT *********************/
        protected override void Start()
        {
            if (animationController.AttackAnimation is MultiAttackAnimationBehaviour behaviour)
            {
                attackAnimationBehaviour = behaviour;
            }
            else
            {
                throw new InvalidCastException("Cast Invalid. Expected: MultiAttackAnimationBehaviour");
            }
            
            base.Start();
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        protected override IEnumerator AttackCoroutine()
        {
            yield return base.AttackCoroutine();
            
            int configurationIndex = 0;

            while (isAlive)
            {
                yield return new WaitForSeconds(patternShootDelay);

                attackAnimationBehaviour.Play(configurationIndex);
                AttackConfiguration configuration = attackConfigurations[configurationIndex];

                Vector3 currentPosition = transform.position;
                configuration.attackBehaviour.Execute(configuration.projectileType, this,
                    () => (player.transform.position - currentPosition).normalized);
                yield return new WaitUntil(() => configuration.attackBehaviour.IsFinished());

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
