using Gameplay;
using UnityEngine;
using Utility;
using Utility.Extensions;

namespace Enemies
{
    public class EnemyDolphin : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private int damage = 1;
        [SerializeField] private float hitDelay = 0.5f;
        
        private Vector2 smoothMoveDirection;
        private float smoothSpeed, velocity;
        
        private bool canAttack = true;
        /**********************************************/
    
        /******************* LOOP *********************/
        private void Update()
        {
            moveDirection = (player.transform.position - transform.position).normalized;
            smoothMoveDirection = Vector2.MoveTowards(smoothMoveDirection, moveDirection, Time.deltaTime);
            transform.localEulerAngles =
                Vector3.forward * (VectorHelper.GetAngleFromDirection(smoothMoveDirection) + 90f);
        }

        private void FixedUpdate()
        {
            smoothSpeed = Mathf.SmoothDamp(smoothSpeed, moveSpeed, ref velocity, Time.deltaTime, moveSpeed);
            body.velocity = smoothMoveDirection * smoothSpeed;
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (canAttack && collision.gameObject.CompareTag("Player") && 
                collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                fireSound.Play(audioSource);
                animationController.AttackAnimation.Play();
                damageable.OnDamaged(damage);

                canAttack = false;
                this.DelayedAction(() => canAttack = true, hitDelay);
            }
        }
        /**********************************************/
    }
}
