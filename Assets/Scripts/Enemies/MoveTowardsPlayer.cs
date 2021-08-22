using UnityEngine;
using Utility.Extensions;

namespace Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveTowardsPlayer : MoveBehaviour
    {
        [SerializeField] private bool doesRotate = false;
        
        private Vector2 smoothMoveDirection;
        private float smoothSpeed, velocity;

        private void Update()
        {
            moveDirection = (player.transform.position - transform.position).normalized;
            smoothMoveDirection = Vector2.MoveTowards(smoothMoveDirection, moveDirection, Time.deltaTime);
            transform.localEulerAngles = doesRotate ? Vector3.forward * (smoothMoveDirection.GetAngle() + 90f) : Vector3.zero;
        }

        private void FixedUpdate()
        {
            smoothSpeed = Mathf.SmoothDamp(smoothSpeed, moveSpeed, ref velocity, Time.deltaTime, moveSpeed);
            rigidbody.velocity = smoothMoveDirection * smoothSpeed;
        }
    }
}