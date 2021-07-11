using System.Collections;
using Gameplay.Projectile;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Enemies
{
    public class EnemyPillar : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private AnimationReferenceAsset idleAnimation;
        [SerializeField] private AnimationReferenceAsset attackAnimation;
        [SerializeField] private AnimationReferenceAsset deathAnimation;
    
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
            currentState = AnimationState.Idle;
            SetAnimation(idleAnimation, true, 1f);
        }
        /**********************************************/
    
        /******************* LOOP *********************/
        private void Update()
        {
            if (moveDistance > 0)
            {
                target = player.transform.position;
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
            SetAnimation(attackAnimation, false, 3f);
        
            Vector3 projectilePosition = transform.position + (Vector3) direction * 2f;
            projectileFactory.Instantiate(ProjectileFactory.ProjectileTypes.EnemyRicochet, projectilePosition, direction);
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
