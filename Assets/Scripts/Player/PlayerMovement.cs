using System.Collections;
using UnityEngine;
using Utility;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        public float speedModifier = 1f, weightModifier = 1f;

        [SerializeField] private float maxSpeed = 50f;
        [SerializeField] private float maxAcceleration = 150f, maxDeceleration = 120f;
        [SerializeField] private float rotationSpeed = 15f;
        
        private new Rigidbody2D rigidbody;
        
        private Vector2 inputDirection;
        private Vector2 velocity, desiredVelocity, lastVelocity;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            inputDirection = Vector2.ClampMagnitude(inputDirection, 1f);
            desiredVelocity = inputDirection * maxSpeed;
            
            float inputAngle = VectorHelper.GetAngleFromDirection(inputDirection);
            float smoothRotationAngle = Mathf.LerpAngle(transform.eulerAngles.z, inputAngle,
                Time.deltaTime * rotationSpeed * inputDirection.magnitude);
            transform.rotation = Quaternion.Euler(0f, 0f, smoothRotationAngle);
        }

        private void FixedUpdate()
        {
            velocity = rigidbody.velocity;

            float acceleration = lastVelocity.magnitude < velocity.magnitude ? maxAcceleration : maxDeceleration;
            float maxSpeedChange = acceleration * weightModifier * speedModifier * Time.deltaTime;
            
            lastVelocity = velocity;
            velocity = Vector2.MoveTowards(velocity, desiredVelocity, maxSpeedChange);
            rigidbody.velocity = velocity;
        }
        
        private IEnumerator SpeedModifierCoroutine(float multiplier, float duration)
        {
            speedModifier = multiplier;

            yield return new WaitForSeconds(duration);
            
            speedModifier = 1f;
        }

        public void ApplySpeedModifier(float multiplier, float duration = 0f)
        {
            StartCoroutine(SpeedModifierCoroutine(multiplier, duration));
        }
        /**********************************************/
    }
}