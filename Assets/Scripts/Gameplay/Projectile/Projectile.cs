using System;
using UnityEngine;
using Utility;

namespace Gameplay.Projectile
{
    /// <summary>
    /// Behaviour class for all types of projectiles in the game.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(TrailRenderer))]
    public class Projectile : MonoBehaviour
    {
        /**************** VARIABLES *******************/
        public ProjectileScriptableObject projectileConfiguration;
        
        [HideInInspector] public Vector2 direction = Vector2.zero;
        [HideInInspector] public int damage;
        [HideInInspector] public float speed;
        
        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private TrailRenderer trailRenderer;
        private Vector2 lastContactNormal;
        private int ricochetAmount;
        /**********************************************/

        /******************* INIT *********************/
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            trailRenderer = GetComponent<TrailRenderer>();
        }

        private void OnEnable()
        {
            damage = projectileConfiguration.damage;
            ricochetAmount = projectileConfiguration.ricochetAmount;
            spriteRenderer.sprite = projectileConfiguration.projectileSprite;
            trailRenderer.colorGradient = projectileConfiguration.trailGradient;
            speed = projectileConfiguration.projectileSpeed;
            transform.eulerAngles = Vector3.forward * VectorHelper.GetAngleFromDirection(direction);
        }
        /**********************************************/

        /******************* LOOP *********************/
        private void FixedUpdate()
        {
            body.velocity = speed * Time.deltaTime * direction;
            transform.eulerAngles = Vector3.forward * VectorHelper.GetAngleFromDirection(direction);
        }
        /**********************************************/

        /***************** METHODS ********************/
        private void OnCollisionEnter2D(Collision2D other)
        {
            HandleCollision(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            HandleCollision(other);
        }

        private void HandleCollision(Collision2D collision)
        {
            Vector2 contactNormal = GetNormal(collision);

            if (contactNormal == lastContactNormal) return;
            
            if (ricochetAmount > 0)
            {
                direction = Vector2.Reflect(direction, contactNormal);
                ricochetAmount--;
                lastContactNormal = contactNormal;
            }
            else
            {
                Disable();
            }

            if (collision.gameObject.TryGetComponent(out IDamageable projectileHit))
            {
                projectileHit.OnDamaged(damage);
            }
        }

        private void Disable()
        {
            gameObject.SetActive(false);
            trailRenderer.Clear();
        }

        private Vector2 GetNormal(Collision2D collision)
        {
            Vector2 normal = Vector2.zero;
            
            for (int i = 0; i < collision.contactCount; i++)
            {
                normal += collision.GetContact(i).normal;
            }

            return normal.normalized;
        }
        /**********************************************/
    }
}