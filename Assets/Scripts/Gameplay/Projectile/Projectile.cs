using UnityEngine;
/// <summary>
/// Behaviour class for all types of projectiles in the game.
/// </summary>
// Note: should this be a base class that's split up between friendly and enemy projectiles, or otherwise differentiated? 
public class Projectile : MonoBehaviour
{
    /**************** VARIABLES *******************/
    public Vector2 direction = Vector3.zero;
    public float playerBulletSpeed = 5f, enemyBulletSpeed = 2f;
    [Space]
    
    private bool playerIsOwner = false;
    public bool PlayerIsOwner
    {
        get => playerIsOwner;
        set
        {
            playerIsOwner = value;
            if (value)
            {
                GetComponent<SpriteRenderer>().sprite = friendlySprite;
                gameObject.layer = 7;
            }
        }
    }
    
    private bool ricochet = false;
    public bool Ricochet
    {
        get => ricochet;
        set
        {
            ricochet = value;
            if (value)
            {
                GetComponent<SpriteRenderer>().sprite = bouncySprite;
            }
        }
    }
    
    [Space]
    [SerializeField] private Sprite friendlySprite;
    [SerializeField] private Sprite enemySprite;
    [SerializeField] private Sprite bouncySprite;
    
    private Rigidbody2D body;
    /**********************************************/
    
    /******************* INIT *********************/
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
    /**********************************************/
    
    /******************* LOOP *********************/
    private void FixedUpdate()
    {
        float speed = playerIsOwner ? playerBulletSpeed : enemyBulletSpeed;
        if (!playerIsOwner & GameManager.Instance.BulletTime > 0)
        {
            speed *= .25f;
        }

        body.velocity += speed * Time.deltaTime * direction;
        transform.localRotation = Quaternion.Euler(0f, 0f, VectorHelper.GetAngleFromDirection(direction));
    }
    /**********************************************/
    
    /***************** METHODS ********************/
    private void OnCollisionEnter2D(Collision2D collision)
    {
            if (Ricochet)
            {
                direction = Vector2.Reflect(direction, collision.transform.right);

                Ricochet = false;
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
