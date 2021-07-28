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
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (ricochetAmount > 0)
            {
                direction = Vector2.Reflect(direction, collision.transform.right);
                ricochetAmount--;
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
        /**********************************************/
    }
}