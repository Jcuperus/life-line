using Gameplay;
using UnityEngine;
using Utility.Extensions;

namespace Enemies
{
    public class EnemyDolphin : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        [SerializeField] private int damage = 1;
        [SerializeField] private float hitDelay = 0.5f;
        
        private bool canAttack = true;
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
