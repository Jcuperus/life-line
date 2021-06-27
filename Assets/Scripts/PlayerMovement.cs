#region namespaces

using System.Collections;
using UnityEngine;

#endregion

/// <summary>
/// Behaviour script for player movement.
/// </summary>
public class PlayerMovement : MonoBehaviour, HealthBarNode, ProjectileHit
{
    /**********************************************/
    /*                VARIABLES                   */
    /**********************************************/
    #region variables
    private Rigidbody2D body;
    public HealthBarNode PreviousNode { get; set; }
    public HealthBarNode NextNode { get; set; }
    [SerializeField] private AudioEven damageSounds;
    [SerializeField] private AudioEven shootingSounds;
    [SerializeField] private AudioEven deathSounds;
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float maxAcceleration = 75f, maxDeceleration = 75f;
    [SerializeField] private float projectileSpawnOffset = 2f;
    [SerializeField] private float attachmentDelay = 1.5f;
    [SerializeField] private int startingHealthAmount = 5;
    [SerializeField] private HealthBarSegment healthBarNodePrefab;
    [SerializeField] private Projectile projectilePrefab;

    public bool inputMode = false;
    public bool mouseAim = false;
    
    private Vector2 inputDir;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Vector2 lastVelocity;
    private bool canAttach = true;
    [SerializeField] private AudioSource audioSource;
    
    #endregion
    /**********************************************/
    /*                   INIT                     */
    /**********************************************/
    #region init
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();

    }
    private void Start()
    {
        for (int i = 0; i < startingHealthAmount; i++)
        {
            SpawnSegment();
        }
    }
    #endregion
    /**********************************************/
    /*                   LOOP                     */
    /**********************************************/
    #region loop
    void Update()
    {
        inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = inputMode ? Vector2.ClampMagnitude(inputDir, 1f) : inputDir.normalized;
        desiredVelocity = inputDir * maxSpeed;

        if (Input.GetButtonDown("Fire1"))
        {
            FireProjectile();
        }

        if (Input.GetButtonDown("Jump"))
        {
            DetachHealthBar();
        }
    }
    void FixedUpdate()
    {
        velocity = body.velocity;

        float acceleration = lastVelocity.magnitude < velocity.magnitude ? maxAcceleration : maxDeceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;
        lastVelocity = velocity;
        velocity = Vector2.MoveTowards(velocity, desiredVelocity, maxSpeedChange);

        body.velocity = velocity;
        // body.velocity = maxSpeed * 10 * speedMultiplier * Time.deltaTime * inputDir;

        if (inputDir.sqrMagnitude > 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, VectorHelper.GetAngleFromDirection(inputDir));
        }
        
    }
    #endregion
    /**********************************************/
    /*                 METHODS                    */
    /**********************************************/
    #region methods
    public void SpawnSegment()
    {
        HealthBarSegment newSegment = Instantiate(healthBarNodePrefab, transform.parent, false);
        AddTail(newSegment);
    }

    private void FireProjectile()
    {
        
        shootingSounds.Play(audioSource);
        Vector3 shootDirection;
        if (mouseAim)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            shootDirection = (mousePosition - transform.position).normalized;
        }
        else
        {
            shootDirection = VectorHelper.GetDirectionFromAngle(transform.eulerAngles.z);
        }

        Projectile projectile = Instantiate(projectilePrefab);
        projectile.PlayerIsOwner = true;
        projectile.transform.position = transform.position + shootDirection * projectileSpawnOffset;

        projectile.direction = shootDirection;
        if (GameManager.Instance.ricochet > 0)
        {
            projectile.ricochet = true;
        }
    }
    
    private void AttachHealthBar(HealthBarNode segment)
    {
        if (!canAttach) return;
        
        HealthBarNode currentSegment = segment;
        
        while (currentSegment.PreviousNode != null)
        {
            currentSegment = currentSegment.PreviousNode;
        }

        NextNode = currentSegment;
        NextNode.PreviousNode = this;
    }
    
    private void DetachHealthBar()
    {
                    canAttach = false;
        if (NextNode != null)
        {
            NextNode.PreviousNode = null;
            NextNode = null;
            PreviousNode = null;
        }


        StartCoroutine(ReenableAttachment());

    }

    private IEnumerator ReenableAttachment()
    {
        yield return new WaitForSeconds(attachmentDelay);
        canAttach = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Pickup pickup))
        {
            PickupType type = pickup.type;
            Destroy(pickup.gameObject);
            GameManager.Instance.ResolvePickup(type);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            PassHit();
            collision.rigidbody.velocity = -collision.rigidbody.velocity;
        }
        else if(NextNode == null && collision.gameObject.TryGetComponent(out HealthBarNode healthBar))
        {
            AttachHealthBar(healthBar);
        }
    }

    public void AddTail(HealthBarNode tail)
    {
        if (NextNode == null)
        {
            NextNode = tail;
            tail.PreviousNode = this;
        }
        else
        {
            NextNode.AddTail(tail);
        }
    }

    public void OnProjectileHit(Projectile projectile)
    {
        PassHit();
    }
    public void PassHit()
    {
        damageSounds.Play(audioSource);
        if (NextNode == null)
        {
            deathSounds.Play(audioSource);
            //Death animation
            GameManager.Instance.Death();
        }
        else
        {
            NextNode.PassHit();
        }
    }

    #endregion
}
