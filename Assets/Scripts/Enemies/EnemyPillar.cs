using System.Collections;
using Gameplay.Projectile;
using UnityEngine;

namespace Enemies
{
    public class EnemyPillar : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [Header("Projectile")]
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float moveDistance = 2f;

        private ProjectileFactory projectileFactory;
        private Vector3 target;
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
            target = player.transform.position;
            if (moveDistance > 0)
            {
                moveDirection = (target - transform.position).normalized;
                moveDistance -= Time.deltaTime;
            }
        }
    
        private void FixedUpdate()
        {
            if (moveDistance > 0)
            {
                body.velocity += moveSpeed * Time.deltaTime * moveDirection;
            }
            else
            {
                body.velocity = Vector2.zero;
            }
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        private void FireBullet(Vector2 direction)
        {
            if (!isAlive) return;
        
            fireSound.Play(audioSource);
            animationController.AttackAnimation.Play(3f);
        
            Vector3 projectilePosition = transform.position + (Vector3) direction * 2f;
            projectileFactory.Instantiate(ProjectileFactory.ProjectileTypes.EnemyRicochet, projectilePosition, direction);
        }
    
        private IEnumerator FireBulletCoroutine()
        {
            while (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(fireRate);
                FireBullet(target);
            }
        }
        /**********************************************/
    }
}
