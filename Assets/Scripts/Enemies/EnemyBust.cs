using System.Collections;
using Gameplay.Projectile;
using UnityEngine;

namespace Enemies
{
    public class EnemyBust : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private float fireRate = 1f;

        private ProjectileFactory projectileFactory;

        private float timer;
        /**********************************************/

        /******************* INIT *********************/
        protected override void Awake()
        {
            base.Awake();
            projectileFactory = ProjectileFactory.Instance;
        }

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

            Vector3 projectilePosition = transform.position + (Vector3) direction * 2f;
            projectileFactory.Instantiate(ProjectileFactory.ProjectileTypes.Enemy, projectilePosition, direction);
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