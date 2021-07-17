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
        public Vector2 direction = Vector2.zero;

        private Rigidbody2D body;
        private bool canRicochet;
        /**********************************************/

        /******************* INIT *********************/
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            canRicochet = projectileConfiguration.canRicochet;

            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = projectileConfiguration.projectileSprite;

            var trailRenderer = GetComponent<TrailRenderer>();
            trailRenderer.colorGradient = projectileConfiguration.trailGradient;
            
            float initialRotation = VectorHelper.GetAngleFromDirection(new Vector3(direction.x, 0f, direction.y));
            transform.eulerAngles = Vector3.up * initialRotation;
        }
        /**********************************************/

        /******************* LOOP *********************/
        private void FixedUpdate()
        {
            float speed = projectileConfiguration.projectileSpeed;
            if (!projectileConfiguration.playerIsOwner & GameManager.Instance.BulletTime > 0)
            {
                speed *= .25f;
            }

            body.velocity = speed * Time.deltaTime * direction;
            transform.localRotation = Quaternion.Euler(0f, 0f, VectorHelper.GetAngleFromDirection(direction));
        }
        /**********************************************/

        /***************** METHODS ********************/
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (canRicochet)
            {
                direction = Vector2.Reflect(direction, collision.transform.right);
                canRicochet = false;
            }
            else
            {
                Destroy(gameObject);
            }

            if (collision.gameObject.TryGetComponent(out IProjectileHit projectileHit))
            {
                projectileHit.OnProjectileHit(this);
            }
        }
        /**********************************************/
    }
}