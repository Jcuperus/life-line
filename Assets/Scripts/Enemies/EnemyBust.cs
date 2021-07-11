using System.Collections;
using Gameplay.Projectile;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Enemies
{
    public class EnemyBust : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private AnimationReferenceAsset idleAnimation;
        [SerializeField] private AnimationReferenceAsset attackAnimation;
        [SerializeField] private AnimationReferenceAsset deathAnimation;

        [Header("Projectile")] [SerializeField]
        private float fireRate = 1f;

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
            currentState = AnimationState.Idle;
            SetAnimation(idleAnimation, true, 1f);
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
        protected override void OnAnimationComplete(TrackEntry trackEntry)
        {
            switch (currentState)
            {
                case AnimationState.Attacking:
                    currentState = AnimationState.Idle;
                    SetAnimation(idleAnimation, true, 1f);
                    break;
                case AnimationState.Death:
                    DestroyEnemy();
                    break;
            }
        }

        private void FireBullet(Vector2 direction)
        {
            if (currentState == AnimationState.Death) return;

            fireSound.Play(audioSource);
            currentState = AnimationState.Attacking;
            SetAnimation(attackAnimation, false, 2f);

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

        public override void OnProjectileHit(Projectile projectile)
        {
            base.OnProjectileHit(projectile);

            if (currentState == AnimationState.Death)
            {
                SetAnimation(deathAnimation, false, 1f);
                canInterruptAnimation = false;
            }
        }
        /**********************************************/
    }
}