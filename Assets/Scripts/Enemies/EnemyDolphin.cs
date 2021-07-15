using UnityEngine;

namespace Enemies
{
    public class EnemyDolphin : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        private float timer;
        /**********************************************/
    
        /******************* LOOP *********************/
        private void Update()
        {
            {
                timer += Time.deltaTime;
                if (timer > 1)
                {
                    timer = 0;
                    Vector3 target = player.transform.position;
                    moveDirection = (target - transform.position).normalized;
                }
            }
        }
    
        private void FixedUpdate()
        {
            body.velocity += moveSpeed * moveSpeed * Time.deltaTime * moveDirection;
        }
        /**********************************************/
    
        /***************** METHODS ********************/
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                fireSound.Play(audioSource);
                animationController.AttackAnimation.Play();
            }
        }
        /**********************************************/
    }
}
