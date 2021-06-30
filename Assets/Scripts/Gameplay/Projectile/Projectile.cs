using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 direction = Vector3.zero;
    public float playerBulletSpeed = 5f, enemyBulletSpeed = 2f;
    private bool playerIsOwner = false;
    private Rigidbody2D body;
    public bool PlayerIsOwner { get => playerIsOwner; 
        set 
        {
            playerIsOwner = value;
            if (value)
            {
                GetComponent<SpriteRenderer>().sprite = friendlySprite;
                gameObject.layer = 7;
            }
        } }
    public bool ricochet
    {
        get => ricochet_;
        set
        {
            ricochet_ = value;
            if (value == true)
            {
                GetComponent<SpriteRenderer>().sprite = bouncySprite;
            }
        }
    }
    private bool ricochet_ = false;
    [SerializeField] private Sprite friendlySprite;
    [SerializeField] private Sprite enemySprite;
    [SerializeField] private Sprite bouncySprite;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        float speed = playerIsOwner ? playerBulletSpeed : enemyBulletSpeed;
        if (!playerIsOwner & GameManager.Instance.BulletTime > 0)
        {
            speed *= .25f;
        }
        body.velocity+=(speed * Time.deltaTime * direction);
        transform.localRotation = Quaternion.Euler(0f, 0f, VectorHelper.GetAngleFromDirection(direction));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            if (ricochet)
            {
                direction = Vector2.Reflect(direction, collision.transform.right);

                ricochet = false;
            }
            else
            {
                Destroy(gameObject);
            }

            if (collision.gameObject.TryGetComponent(out ProjectileHit projectileHit))
            { 
                projectileHit.OnProjectileHit(this);
            }
    }
}
