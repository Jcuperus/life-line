using System.Collections;
using Gameplay.AttackBehaviour;
using Gameplay.Projectile;
using UnityEngine;

namespace Enemies
{
    public class EnemyPillar : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private FireBehaviour fireBehaviour;
        [SerializeField] private float fireRate = 1f;
        /**********************************************/
    
        /******************* INIT *********************/
        private void Start()
        {
            StartCoroutine(FireBulletCoroutine());
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        private void FireBullet(Vector2 direction)
        {
            if (!isAlive) return;
        
            fireSound.Play(audioSource);
            animationController.AttackAnimation.Play(3f);
            fireBehaviour.Execute(ProjectileFactory.ProjectileTypes.EnemyRicochet, this, direction);
        }
    
        private IEnumerator FireBulletCoroutine()
        {
            while (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(fireRate);
                Vector3 direction = (player.transform.position - transform.position).normalized;
                FireBullet(direction);
            }
        }
        /**********************************************/
    }
}
