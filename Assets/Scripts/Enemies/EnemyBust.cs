using System.Collections;
using Gameplay.AttackBehaviour;
using Gameplay.Projectile;
using UnityEngine;

namespace Enemies
{
    public class EnemyBust : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private FireBehaviour fireBehaviour;
        [SerializeField] private float fireRate = 1f;

        private float timer;
        /**********************************************/

        /******************* INIT *********************/
        private void Start()
        {
            StartCoroutine(FireBulletCoroutine());
        }
        /**********************************************/

        /******************* LOOP *********************/
        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > 1)
            {
                timer = 0;
                Vector3 target = player.transform.position;
                moveDirection = (target - transform.position).normalized;
            }
        }

        private void FixedUpdate()
        {
            body.velocity += moveSpeed * Time.deltaTime * moveDirection;
        }
        /**********************************************/

        /***************** METHODS ********************/
        private void FireBullet(Vector2 direction)
        {
            if (!isAlive) return;

            fireSound.Play(audioSource);
            animationController.AttackAnimation.Play(3f);
            fireBehaviour.Execute(ProjectileFactory.ProjectileTypes.Enemy, this, direction);
        }

        private IEnumerator FireBulletCoroutine()
        {
            while (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(fireRate);
                FireBullet(moveDirection);
            }
        }
        /**********************************************/
    }
}