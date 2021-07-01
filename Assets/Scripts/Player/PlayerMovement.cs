using System.Collections.Generic;
using Player.HealthBar;
using UnityEngine;

/// <summary>
/// Behaviour script for player movement.
/// </summary>
[RequireComponent(typeof(HealthBar))]
public class PlayerMovement : MonoBehaviour, IProjectileHit
{
    /**************** VARIABLES *******************/
    [SerializeField] private int startingHealthAmount = 5;
    
    [Header("Movement Parameters")]
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float maxAcceleration = 75f, maxDeceleration = 75f;
    [SerializeField] private float projectileSpawnOffset = 2f;

    [Header("Settings")]
    public bool inputMode = false;
    public bool mouseAim = false;
    
    [Header("Prefabs & Assets")]
    [SerializeField] private AudioEvent damageSounds;
    [SerializeField] private AudioEvent shootingSounds;
    [SerializeField] private AudioEvent deathSounds;
    
    [Space]
    [SerializeField] private HealthBarSegment healthBarNodePrefab;
    [SerializeField] private Projectile projectilePrefab;

    private Rigidbody2D body;
    private AudioSource audioSource;
    
    private HealthBar healthBar;
    private LinkedListNode<GameObject> Node { get; set; }
    
    private Vector2 inputDir;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Vector2 lastVelocity;
    /**********************************************/
    
    /******************* INIT *********************/
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        Node = new LinkedListNode<GameObject>(gameObject);
        healthBar = GetComponent<HealthBar>();
        healthBar.AddFirst(Node);
    }
    
    private void Start()
    {
        for (int i = 0; i < startingHealthAmount; i++)
        {
            SpawnSegment();
        }
    }
    /**********************************************/
    
    /******************* LOOP *********************/
    private void Update()
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
    
    private void FixedUpdate()
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
    /**********************************************/
    
    /***************** METHODS ********************/
    public void SpawnSegment()
    {
        HealthBarSegment newSegment = Instantiate(healthBarNodePrefab, transform.parent, false);
        healthBar.AddLast(newSegment.Node);
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
        if (GameManager.Instance.Ricochet > 0)
        {
            projectile.Ricochet = true;
        }
    }
    
    private void DetachHealthBar()
    {
        if (healthBar.IsFirst(Node)) healthBar.RemoveFirst();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Pickup pickup))
        {
            PickupType type = pickup.Type;
            Destroy(pickup.gameObject);
            GameManager.Instance.ResolvePickup(type);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnDamage();
            collision.rigidbody.velocity = -collision.rigidbody.velocity;
        }
        else if(collision.gameObject.CompareTag("HealthBar") && !healthBar.IsFirst(Node))
        {
            healthBar.AddFirst(Node);
        }
    }
    
    public void OnProjectileHit(Projectile projectile)
    {
        OnDamage();
    }

    private void OnDamage()
    {
        damageSounds.Play(audioSource);

        if (healthBar.Count > 1)
        {
            deathSounds.Play(audioSource);
            GameManager.Instance.Death();
        }
        else
        {
            healthBar.RemoveLast();
        }
        
        healthBar.RemoveLast();
    }
    /**********************************************/
}
