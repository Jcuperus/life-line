using UnityEngine;
using Utility;

namespace Enemies
{
    public class EnemyDolphin : AbstractEnemy
    {
        /**************** VARIABLES *******************/
        private float smoothSpeed, velocity;
        private Vector2 smoothMoveDirection;
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
            if (collision.gameObject.CompareTag("Player"))
            {
                fireSound.Play(audioSource);
                animationController.AttackAnimation.Play();
            }
        }
        /**********************************************/
    }
}
